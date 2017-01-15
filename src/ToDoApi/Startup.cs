using System;
using Darker;
using Darker.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.messagestore.sqlite;
using paramore.brighter.commandprocessor.messaginggateway.rmq;
using paramore.brighter.commandprocessor.messaginggateway.rmq.MessagingGatewayConfiguration;
using Polly;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;
using ToDoCore.Adaptors;
using ToDoCore.Adaptors.BrighterFactories;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Mappers;
using ToDoCore.Ports.Queries;
using HandlerConfiguration = paramore.brighter.commandprocessor.HandlerConfiguration;
using InMemoryRequestContextFactory = paramore.brighter.commandprocessor.InMemoryRequestContextFactory;
using PolicyRegistry = paramore.brighter.commandprocessor.PolicyRegistry;

namespace ToDoApi
{
    public partial class Startup
    {
        private readonly Container _container ;
        public Startup(IHostingEnvironment env)
        {
            //use a sensible constructor resolution approach
            _container = new Container();
            _container.Options.ConstructorResolutionBehavior = new MostResolvableConstructorBehavior(_container);

            BuildConfiguration(env);
        }

        private void BuildConfiguration(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set;  }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ToDoContext>(options => options.UseSqlite("Data Source=./ToDoDb.sqlite"));

            services.AddMvc();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            app.UseSimpleInjectorAspNetRequestScoping(_container);
            _container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            InitializeContainer(app);
            _container.Verify();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("AllowAll");

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();

            EnsureDatabaseCreated();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            RegisterCommandProcessor();

            RegisterQueryProcessor();

            // Cross-wire ASP.NET services (if any). For instance:
            _container.RegisterSingleton(app.ApplicationServices.GetService<ILoggerFactory>());
            _container.RegisterSingleton(app.ApplicationServices.GetService<DbContextOptions<ToDoContext>>());
            // NOTE: Prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
        }

        private void RegisterQueryProcessor()
        {
            var registry = new QueryHandlerRegistry();
            registry.Register<ToDoByIdQuery, ToDoByIdQuery.Result, ToDoByIdQueryHandlerAsync>();
            registry.Register<ToDoQueryAll, ToDoQueryAll.Result, ToDoQueryAllHandlerAsync>();

            _container.Register<IQueryHandler<ToDoByIdQuery, ToDoByIdQuery.Result>, ToDoByIdQueryHandlerAsync>(Lifestyle.Scoped);
            _container.Register<IQueryHandler<ToDoQueryAll, ToDoQueryAll.Result>, ToDoQueryAllHandlerAsync>(Lifestyle.Scoped);

            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
             var policyRegistry = new Darker.PolicyRegistry {{QueryProcessor.RetryPolicyName, retryPolicy}, {QueryProcessor.CircuitBreakerPolicyName, circuitBreakerPolicy}};

            Func<Type, object> simpleFactory = type =>  _container.GetInstance(type);

            IQueryProcessor queryProcessor = QueryProcessorBuilder.With()
                .Handlers(registry, simpleFactory, Activator.CreateInstance)
                .Policies(policyRegistry)
                .InMemoryRequestContextFactory()
                .Build();
            _container.RegisterSingleton<IQueryProcessor>(queryProcessor);

        }


        private void RegisterCommandProcessor()
        {
            //create handler 
            var subscriberRegistry = new SubscriberRegistry();
            _container.Register<IHandleRequestsAsync<AddToDoCommand>, AddToDoCommandHandlerAsync>(Lifestyle.Scoped);
            _container.Register<IHandleRequestsAsync<DeleteAllToDosCommand>, DeleteAllToDosCommandHandlerAsync>(Lifestyle.Scoped);
            _container.Register<IHandleRequestsAsync<DeleteToDoByIdCommand>, DeleteToDoByIdCommandHandlerAsync>(Lifestyle.Scoped);
            _container.Register<IHandleRequestsAsync<UpdateToDoCommand>, UpdateToDoCommandHandlerAsync>(Lifestyle.Scoped);

            subscriberRegistry.RegisterAsync<AddToDoCommand, AddToDoCommandHandlerAsync>();
            subscriberRegistry.RegisterAsync<DeleteAllToDosCommand, DeleteAllToDosCommandHandlerAsync>();
            subscriberRegistry.RegisterAsync<DeleteToDoByIdCommand, DeleteToDoByIdCommandHandlerAsync>();
            subscriberRegistry.RegisterAsync<UpdateToDoCommand, UpdateToDoCommandHandlerAsync>();

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

            var servicesHandlerFactory = new ServicesHandlerFactoryAsync(_container);

            var messagingGatewayConfiguration = RmqGatewayBuilder.With.Uri(new Uri("amqp://guest:guest@localhost:5672/%2f")).Exchange("future.stack.exchange").DefaultQueues();

            var gateway = new RmqMessageProducer(messagingGatewayConfiguration);
            var sqlMessageStore = new SqliteMessageStore(new SqliteMessageStoreConfiguration("Data Source = messages.sqlite", "Messages"));

            var messageMapperFactory = new MessageMapperFactory(_container);
            _container.Register<IAmAMessageMapper<BulkAddToDoCommand>, BulkAddToDoMessageMapper>();

            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(BulkAddToDoCommand), typeof(BulkAddToDoMessageMapper)}
            };

            var messagingConfiguration = new MessagingConfiguration(
                messageStore: sqlMessageStore,
                messageProducer: gateway,
                messageMapperRegistry: messageMapperRegistry);

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(subscriberRegistry, servicesHandlerFactory))
                .Policies(policyRegistry)
                .TaskQueues(messagingConfiguration)
                .RequestContextFactory(new InMemoryRequestContextFactory())
                .Build();

            _container.RegisterSingleton<IAmACommandProcessor>(commandProcessor);
        }

        private void EnsureDatabaseCreated()
        {
            var contextOptions = _container.GetInstance<DbContextOptions<ToDoContext>>();
            using (var context = new ToDoContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }
        }
    }
}


