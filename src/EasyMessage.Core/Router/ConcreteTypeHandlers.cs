using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyMessage.Core.Messages;

namespace EasyMessage.Core.Router
{
    internal class ConcreteTypeHandlers
    {
        public List<Type> ExpandedTypes { get; }
        public Dictionary<object, Action<IMessage>> Handlers { get; } = new Dictionary<object, Action<IMessage>>();

        public readonly MethodInfo AddHandlerInfo;
        public readonly MethodInfo RemoveHandlerInfo;


        public ConcreteTypeHandlers(Type type)
        {
            ExpandedTypes = new List<Type>() { type };

            // ReSharper disable once PossibleNullReferenceException
            AddHandlerInfo = GetType().GetMethod(nameof(AddHandler)).MakeGenericMethod(type);
            // ReSharper disable once PossibleNullReferenceException
            RemoveHandlerInfo = GetType().GetMethod(nameof(RemoveHandler)).MakeGenericMethod(type);
        }

        public void AddHandler<T>(IHandleMessage<T> handler) where T : IMessage
        {
            var handlerType = handler.GetType();
            if (Handlers.ContainsKey(handlerType)) return;
            void Delegated(IMessage x) => handler.HandleMessage((T)x);
            Handlers[handlerType] = Delegated;
        }

        public void RemoveHandler<T>(IHandleMessage<T> handler) where T : IMessage
        {
            var handlerType = handler.GetType();
            if (!Handlers.TryGetValue(handlerType, out _)) return;
            Handlers.Remove(handlerType);
        }

        public MessageActions ToMessageActions()
        {
            var actions = Handlers.Select(kv => new TaggedAction<IMessage>(kv.Key.ToString(), kv.Value)).ToArray();
            return new MessageActions(actions);
        }
    }
}