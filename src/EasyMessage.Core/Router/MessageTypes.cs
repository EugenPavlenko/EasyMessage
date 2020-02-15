using System;
using System.Collections.Generic;
using System.Linq;
using EasyMessage.Core.Utils;

namespace EasyMessage.Core.Router
{
    /// <summary>
    /// List of Message types
    /// </summary>
    internal class MessageTypes
    {
        private readonly Type handlerType;

        /// <summary>
        /// Concrete types for IHandleMessage
        /// </summary>
        public IEnumerable<Type> MessageTypesList { get; private set; }

        public MessageTypes(Type handlerType)
        {
            this.handlerType = handlerType;
            ExtractHandlers();
        }

        private void ExtractHandlers()
        {
            var interfaces = ReflectionUtils.GetAllInterfaces(handlerType).ToList();
            MessageTypesList = ReflectionUtils.FilterInterfacesForGenericType(interfaces, typeof(IHandleMessage<>))
                .Select(x => x.GenericTypeArguments[0]);
        }
    }
}