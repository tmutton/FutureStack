using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.ServiceActivator;
using Polly;
using Serilog;
using Serilog.Events;
using ToDoGitterApp.Ports;

namespace ToDoGitterApp
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set;  }


        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger();

            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

           Configuration = builder.Build();


            MessageMapperRegistry messageMapperRegistry;
            HandlerConfiguration handlerConfiguration;
            var serviceProvider = ServiceProvider(serviceCollection, out messageMapperRegistry, out handlerConfiguration);

            var rmqConnnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri(Configuration["RabbitMQ:Uri"])),
                Exchange = new Exchange(Configuration["RabbitMQ:Exchange"]),
            };

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(handlerConfiguration)
                .Policies(PolicyRegistry())
                .NoTaskQueues()
                .RequestContextFactory(new InMemoryRequestContextFactory())
                .Build();

            var dispatcher = DispatchBuilder.With()
                .CommandProcessor(commandProcessor)
                .MessageMappers(messageMapperRegistry)
                .DefaultChannelFactory(new InputChannelFactory(new RmqMessageConsumerFactory(rmqConnnection), new RmqMessageProducerFactory(rmqConnnection)))
                .Connections(new Connection[]
                {
                    new Connection<TaskCompletedEvent>(
                        new ConnectionName("TaskCompletedEvent"),
                        new ChannelName("taskcompleted.event"),
                        new RoutingKey("taskcompleted.event"),
                        timeoutInMilliseconds: 200)
                }).Build();

            dispatcher.Receive();

            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();


            dispatcher.End().Wait();
            commandProcessor.Dispose();
            serviceProvider.Dispose();
        }

        private static PolicyRegistry PolicyRegistry()
        {
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

            var policyRegistry = new PolicyRegistry
            {
                {CommandProcessor.RETRYPOLICY, retryPolicy},
                {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
            };
            return policyRegistry;
        }

        private static ServiceProvider ServiceProvider(ServiceCollection serviceCollection,
            out MessageMapperRegistry messageMapperRegistry, out HandlerConfiguration handlerConfiguration)
        {
            serviceCollection.AddTransient<IHandleRequests<TaskCompletedEvent>, TaskUpdateEventHandler>();
            serviceCollection.AddTransient<IAmAMessageMapper<TaskCompletedEvent>, TaskCompleteEventMessageMapper>();

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.Register<TaskCompletedEvent, TaskUpdateEventHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var handlerFactory = new ServiceProviderHandlerFactory(serviceProvider);
            var messageMapperFactory = new ServiceProviderMessageMapperFactory(serviceProvider);

            messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(TaskCompletedEvent), typeof(TaskCompleteEventMessageMapper)}
            };


            handlerConfiguration = new HandlerConfiguration(subscriberRegistry, handlerFactory);
            return serviceProvider;
        }
    }
}