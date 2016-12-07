using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paramore.brighter.commandprocessor;
using Polly;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;

namespace FutureStack
{
    public class Startup
    {
        private Container container = new Container();
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ToDoContext>(options =>
                    options.UseSqlite("Data Source=ToDoDb.sqlite"));

            services.AddMvc();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSimpleInjectorAspNetRequestScoping(container);
            container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();
            InitializeContainer(app);
            container.Verify();


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            app.UseCors("AllowAll");

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            container.RegisterMvcControllers(app);
            container.RegisterMvcViewComponents(app);

            RegisterCommandProcessor();

            // Cross-wire ASP.NET services (if any). For instance:
            container.RegisterSingleton(app.ApplicationServices.GetService<ILoggerFactory>());
            container.RegisterSingleton(app.ApplicationServices.GetService<DbContextOptions<ToDoContext>>());
            // NOTE: Prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
        }


        private void RegisterCommandProcessor()
        {
            //create handler 
            var subscriberRegistry = new SubscriberRegistry();
            container.Register<IHandleRequests<AddToDoCommand>, AddToDoCommandHandler>(Lifestyle.Scoped);
            subscriberRegistry.Register<AddToDoCommand, AddToDoCommandHandler>();

            //create policies
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new PolicyRegistry() { { CommandProcessor.RETRYPOLICY, retryPolicy }, { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy } };

            var servicesHandlerFactory = new ServicesHandlerFactory(container);

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(subscriberRegistry, servicesHandlerFactory))
                .Policies(policyRegistry)
                .NoTaskQueues()
                .RequestContextFactory(new InMemoryRequestContextFactory())
                .Build();

            container.RegisterSingleton<IAmACommandProcessor>(commandProcessor);
        }

        private class ServicesHandlerFactory : IAmAHandlerFactory
        {
            private readonly Container _serviceProvider;

            public ServicesHandlerFactory(Container serviceProvider)
            {
                _serviceProvider = serviceProvider;

            }
            public IHandleRequests Create(Type handlerType)
            {
                return _serviceProvider.GetInstance(handlerType) as IHandleRequests;
            }

            public void Release(IHandleRequests handler)
            {
                var disposable = handler as IDisposable;
                disposable?.Dispose();
            }
        }
    }
}


