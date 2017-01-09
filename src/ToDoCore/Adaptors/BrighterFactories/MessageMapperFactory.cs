using System;
using paramore.brighter.commandprocessor;
using SimpleInjector;

namespace ToDoCore.Adaptors.BrighterFactories
{
    public class MessageMapperFactory : IAmAMessageMapperFactory
    {
        private readonly Container _serviceProvider;


        public MessageMapperFactory(Container serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAmAMessageMapper Create(Type messageMapperType)
        {
           return _serviceProvider.GetInstance(messageMapperType) as IAmAMessageMapper;
        }
    }
}