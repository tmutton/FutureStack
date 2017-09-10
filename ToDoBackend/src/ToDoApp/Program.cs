using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Paramore.Brighter;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.ServiceActivator;
using Polly;
using Serilog;
using SimpleInjector;
using ToDoCore.Adaptors;
using ToDoCore.Adaptors.BrighterFactories;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Mappers;

namespace ToDoApp
{
    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set;  }

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var container = new Container();
            container.Options.ConstructorResolutionBehavior = new MostResolvableParametersConstructorResolutionBehavior(container);

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

           Configuration = builder.Build();

            //Database - this won't work, as its not the same Db as the web site, we should switch to Sql Server here
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseMySql(Configuration["Database:ToDo"])
                .Options;

            container.Register(() => options);

            //Exchange
            var rmqConnnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri(Configuration["RabbitMQ:Uri"])),
                Exchange = new Exchange(Configuration["RabbitMQ:Exchange"]),
            };

            var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection);
            var rmqMessageProducerFactory = new RmqMessageProducerFactory(rmqConnnection);

            // Channels (Message Routing)
            var connections = new List<Connection>
            {
                new Connection<BulkAddToDoCommand>(
                    new ConnectionName("future.stack.todo"),
                    new ChannelName("bulkaddtodo.command"),
                    new RoutingKey("bulkaddtodo.command"),
                    timeoutInMilliseconds: 200,
                    isAsync: true)
            };

            var dispatcher = CreateDispatcher(container, connections, rmqMessageConsumerFactory, rmqMessageProducerFactory);

            CheckDbIsUp(Configuration["Database:ToDoDb"]);
            //Must call once all container registrations complete, as cannnot register post first get.
            EnsureDatabaseCreated(container);

            dispatcher.Receive();

            Console.WriteLine("Press Enter to stop ...");
            Console.ReadLine();

            dispatcher.End().Wait();
        }

        private static void CheckDbIsUp(string connectionString)
        {
            var policy = Policy.Handle<MySqlException>().WaitAndRetryForever(
                retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timespan) =>
                {
                    Console.WriteLine($"Healthcheck: Waiting for the database {connectionString} to come online - {exception.Message}");
                });

            policy.Execute(() =>
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                }
            });
        }

        private static Dispatcher CreateDispatcher(
            Container container,
            List<Connection> connections, RmqMessageConsumerFactory rmqMessageConsumerFactory,
            RmqMessageProducerFactory rmqMessageProducerFactory
        )
        {
            var handlerFactoryAsync = new ServicesHandlerFactoryAsync(container);
            container.Register<IHandleRequestsAsync<BulkAddToDoCommand>, BulkAddToDoCommandHandlerAsync>();
            var messageMapperFactory = new MessageMapperFactory(container);
            container.Register<IAmAMessageMapper<BulkAddToDoCommand>, BulkAddToDoMessageMapper>();

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.RegisterAsync<BulkAddToDoCommand, BulkAddToDoCommandHandlerAsync>();

            //create policies
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
             var retryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicyAsync = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new PolicyRegistry()
            {
                { CommandProcessor.RETRYPOLICY, retryPolicy },
                { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
                { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
                { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync }
            };

            //create message mappers
            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(BulkAddToDoCommand), typeof(BulkAddToDoMessageMapper)}
            };

            var builder = DispatchBuilder
                .With()
                .CommandProcessor(CommandProcessorBuilder.With()
                    .Handlers(new HandlerConfiguration(subscriberRegistry, handlerFactoryAsync))
                    .Policies(policyRegistry)
                    .NoTaskQueues()
                    .RequestContextFactory(new InMemoryRequestContextFactory())
                    .Build()
                )
                .MessageMappers(messageMapperRegistry)
                .DefaultChannelFactory(new InputChannelFactory(rmqMessageConsumerFactory, rmqMessageProducerFactory))
                .Connections(connections);

            var dispatcher = builder.Build();
            return dispatcher;
        }


        private static void EnsureDatabaseCreated(Container container)
        {
            

            var policy = Policy.Handle<MySqlException>().WaitAndRetryForever(
                retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timespan) =>
                {
                    Console.WriteLine($"Can't EnsureCreated waiting for the database to come online - {exception.Message}");
                });

            policy.Execute(() =>
            {
                var contextOptions = container.GetInstance<DbContextOptions<ToDoContext>>();
                using (var context = new ToDoContext(contextOptions))
                {
                    context.Database.EnsureCreated();
                }
            });
        }
    }
}
