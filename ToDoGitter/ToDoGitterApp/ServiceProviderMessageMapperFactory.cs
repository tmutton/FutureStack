using System;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using SimpleInjector;

namespace ToDoGitterApp
{
    internal class ServiceProviderMessageMapperFactory : IAmAMessageMapperFactory
    {
        private readonly Container _serviceProvider;

        public ServiceProviderMessageMapperFactory(Container serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAmAMessageMapper Create(Type messageMapperType)
        {
            return _serviceProvider.GetInstance(messageMapperType) as IAmAMessageMapper;
        }
    }
}