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
using SimpleInjector;
using ToDoGitterApp.Ports;

namespace ToDoGitterApp
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }


        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger();

            var container = new Container();

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            Configuration = builder.Build();


            var serviceProvider = ServiceProvider(container, out var messageMapperRegistry, out var handlerConfiguration);

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

        private static Container ServiceProvider(Container container,
            out MessageMapperRegistry messageMapperRegistry, out HandlerConfiguration handlerConfiguration)
        {
            container.Register<IHandleRequests<TaskCompletedEvent>, TaskCompletedEventHandler>();
            container.Register<IAmAMessageMapper<TaskCompletedEvent>, TaskCompletedEventMessageMapper>();

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.Register<TaskCompletedEvent, TaskCompletedEventHandler>();


            var handlerFactory = new ServiceProviderHandlerFactory(container);
            var messageMapperFactory = new ServiceProviderMessageMapperFactory(container);

            messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(TaskCompletedEvent), typeof(TaskCompletedEventMessageMapper)}
            };


            handlerConfiguration = new HandlerConfiguration(subscriberRegistry, handlerFactory);
            return container;
        }
    }
}