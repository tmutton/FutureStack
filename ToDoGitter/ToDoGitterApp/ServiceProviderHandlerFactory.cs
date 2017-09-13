using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using SimpleInjector;

namespace ToDoGitterApp
{
    internal class ServiceProviderHandlerFactory : IAmAHandlerFactory
    {
        private readonly Container _serviceProvider;

        public ServiceProviderHandlerFactory(Container serviceProvider)
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