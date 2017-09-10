using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;

namespace ToDoGitterApp
{
    internal class ServiceProviderMessageMapperFactory : IAmAMessageMapperFactory
    {
        private readonly ServiceProvider _serviceProvider;

        public ServiceProviderMessageMapperFactory(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAmAMessageMapper Create(Type messageMapperType)
        {
            return _serviceProvider.GetService(messageMapperType) as IAmAMessageMapper;
        }
    }
}