using System;
using paramore.brighter.commandprocessor;
using SimpleInjector;

namespace ToDoCore.Adaptors.BrighterFactories
{
    public class ServicesHandlerFactoryAsync : IAmAHandlerFactoryAsync
    {
        private readonly Container _serviceProvider;

        public ServicesHandlerFactoryAsync(Container serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public IHandleRequestsAsync Create(Type handlerType)
        {
            return _serviceProvider.GetInstance(handlerType) as IHandleRequestsAsync;
        }

        public void Release(IHandleRequestsAsync handler)
        {
            var disposable = handler as IDisposable;
            disposable?.Dispose();
        }
    }
}