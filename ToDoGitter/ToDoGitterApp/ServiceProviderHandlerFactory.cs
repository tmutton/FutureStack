using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace ToDoGitterApp
{
    internal class ServiceProviderHandlerFactory : IAmAHandlerFactory
    {
        private readonly ServiceProvider _serviceProvider;

        public ServiceProviderHandlerFactory(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHandleRequests Create(Type handlerType)
        {
            return _serviceProvider.GetService(handlerType) as IHandleRequests;
        }

        public void Release(IHandleRequests handler)
        {
            var disposable = handler as IDisposable;
            disposable?.Dispose();
        }
    }
}