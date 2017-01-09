using System;
using System.Collections.Generic;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.messaginggateway.rmq;
using paramore.brighter.commandprocessor.messaginggateway.rmq.MessagingGatewayConfiguration;
using paramore.brighter.serviceactivator;
using Polly;
using Serilog;
using SimpleInjector;
using ToDoCore.Adaptors;
using ToDoCore.Adaptors.BrighterFactories;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Mappers;

namespace ToDoApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var container = new Container();
            container.Options.ConstructorResolutionBehavior = new MostResolvableConstructorBehavior(container);


            var handlerFactory = new ServicesHandlerFactoryAsync(container);
            container.Register<IHandleRequestsAsync<BulkAddToDoCommand>, BulkAddToDoCommandHandlerAsync>();
            var messageMapperFactory = new MessageMapperFactory(container);
            container.Register<IAmAMessageMapper<BulkAddToDoCommand>, BulkAddToDoMessageMapper>();

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.RegisterAsync<BulkAddToDoCommand, BulkAddToDoCommandHandlerAsync>();

            //create policies
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(150)
                });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

            var policyRegistry = new PolicyRegistry()
            {
                {CommandProcessor.RETRYPOLICY, retryPolicy},
                {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
            };

            //create message mappers
            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(BulkAddToDoCommand), typeof(BulkAddToDoMessageMapper)}
            };

            //create the gateway
            var rmqConnnection = new RmqMessagingGatewayConnection
            {
                AmpqUri  = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672/%2f")),
                Exchange = new Exchange("future.stack.exchange"),
            };

            var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection );
            var rmqMessageProducerFactory = new RmqMessageProducerFactory(rmqConnnection );

            // Service Activator connections
            var connections = new List<paramore.brighter.serviceactivator.Connection>
            {
                new paramore.brighter.serviceactivator.Connection(
                    new ConnectionName("future.stack.todo"),
                    new InputChannelFactory(rmqMessageConsumerFactory, rmqMessageProducerFactory),
                    typeof(BulkAddToDoCommand),
                    new ChannelName("bulkaddtodo.command"),
                    "bulkaddtodo.command",
                    timeoutInMilliseconds: 200)
            };

            var builder = DispatchBuilder
                .With()
                .CommandProcessor(CommandProcessorBuilder.With()
                    .Handlers(new HandlerConfiguration(subscriberRegistry, handlerFactory))
                    .Policies(policyRegistry)
                    .NoTaskQueues()
                    .RequestContextFactory(new InMemoryRequestContextFactory())
                    .Build()
                )
                .MessageMappers(messageMapperRegistry)
                .ChannelFactory(new InputChannelFactory(rmqMessageConsumerFactory, rmqMessageProducerFactory))
                .Connections(connections);

            var dispatcher = builder.Build();


            dispatcher.Receive();

            Console.WriteLine("Press Enter to stop ...");
            Console.ReadLine();

            dispatcher.End().Wait();
        }

    }
}
