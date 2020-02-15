using System;
using System.Collections.Generic;
using EasyMessage.Core.Extension;
using EasyMessage.Core.Messages;
using EasyMessage.Core.Utils;

namespace EasyMessage.Core.Router
{
    public sealed class MessageRouter : IRouter
    {
        private readonly Dictionary<Type, MessageTypes> handlerExtractors =
            new Dictionary<Type, MessageTypes>();

        private readonly Dictionary<Type, ConcreteTypeHandlers> concreteTypeToHandlers =
            new Dictionary<Type, ConcreteTypeHandlers>();

        private readonly Dictionary<Type, MessageActions> msgActionsCache =
            new Dictionary<Type, MessageActions>();

        private readonly object cacheLock = new object();

        /// <inheritdoc cref="IRouter.Register"/>
        public void Register(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var handlerType = instance.GetType();
            if (!handlerType.IsContainInterface(typeof(IHandleMessage<>)))
                throw new ArgumentException("Wrong handler instance type");
            RegisterHandlerType(handlerType, instance);
        }

        private void RegisterHandlerType(Type handlerType, object handlerInstance)
        {
            MessageTypes messageTypes;
            lock (cacheLock)
            {
                if (!handlerExtractors.TryGetValue(handlerType, out messageTypes))
                {
                    handlerExtractors[handlerType] = messageTypes = new MessageTypes(handlerType);
                }

                foreach (var type in messageTypes.MessageTypesList)
                {
                    RegisterHandler(type, handlerInstance);
                }

                msgActionsCache.Clear();
            }
        }

        private void RegisterHandler(Type type, object instance)
        {
            var typeInfo = GetConcreteTypeHandlers(type);
            foreach (var expandedType in typeInfo.ExpandedTypes)
            {
                AddHandlerToType(concreteTypeToHandlers[expandedType], instance);
            }
        }

        /// <inheritdoc cref="IRouter.UnRegister"/>
        public void UnRegister(object instance)
        {
            var handlerType = instance.GetType();
            UnRegisterHandlerType(handlerType, instance);
        }

        private void UnRegisterHandlerType(Type handlerType, object handlerInstance)
        {
            lock (cacheLock)
            {
                if (!handlerExtractors.TryGetValue(handlerType, out var messageTypes))
                {
                    return;
                }

                handlerExtractors.Remove(handlerType);

                foreach (var type in messageTypes.MessageTypesList)
                {
                    UnRegisterHandler(type, handlerInstance);
                }
                msgActionsCache.Clear();
            }

        }

        private void UnRegisterHandler(Type type, object instance)
        {
            var typeInfo = GetConcreteTypeHandlers(type);
            foreach (var expandedType in typeInfo.ExpandedTypes)
            {
                RemoveHandlerFromType(concreteTypeToHandlers[expandedType], instance);
            }
        }


        /// <inheritdoc cref="IRouter.GetHandlers"/>
        public IEnumerable<TaggedAction<IMessage>> GetHandlers(IMessage message)
        {
            return GetMessageActionsForType(message.GetType()).Actions;
        }

        private MessageActions GetMessageActionsForType(Type type)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (msgActionsCache.TryGetValue(type, out var actions)) return actions;
            lock (cacheLock)
            {
                if (msgActionsCache.TryGetValue(type, out actions)) return actions;

                var typeInfo = concreteTypeToHandlers.GetOrNull(type);
                if (typeInfo == null)
                {
                    CacheMessageType(type);
                    typeInfo = concreteTypeToHandlers[type];
                }
                actions = typeInfo.ToMessageActions();

                msgActionsCache[type] = actions;
            }
            return actions;
        }

        private static void AddHandlerToType(ConcreteTypeHandlers typeHandlers, object handler)
        {
            typeHandlers.AddHandlerInfo?.Invoke(typeHandlers, new[] { handler });
        }

        private static void RemoveHandlerFromType(ConcreteTypeHandlers typeHandlers, object handler)
        {
            typeHandlers.RemoveHandlerInfo.Invoke(typeHandlers, new[] { handler });
        }

        private ConcreteTypeHandlers GetConcreteTypeHandlers(Type type)
        {
            ConcreteTypeHandlers info;
            // ReSharper disable once InconsistentlySynchronizedField
            if (!concreteTypeToHandlers.TryGetValue(type, out info))
            {
                lock (this)
                {
                    if (!concreteTypeToHandlers.TryGetValue(type, out info))
                    {
                        CacheMessageType(type);
                    }

                    info = concreteTypeToHandlers[type];
                }
            }

            return info;
        }

        /// <summary>
        /// Caches a message type and its parent type.
        /// Must be called while cacheLock
        /// </summary>
        private void CacheMessageType(Type type)
        {
            var info = new ConcreteTypeHandlers(type);
            concreteTypeToHandlers[type] = info;


            if (typeof(IMessage).IsAssignableFrom(type))
            {

                //ToDo: Add log
                //log.WarnFormat("Concrete type {0} does not have a matching MessageType and will not be handeled externally", type);
            }

            // Loop until the IMessage parent root, then travel back down adding listeners to each type.
            // This is done to avoid needing to check type parents when messages arrive.


            var parentClassesAdded = new List<ConcreteTypeHandlers>();
            var intermediateTypes = new List<Type>();
            ConcreteTypeHandlers firstParentInCache = null;
            var parent = type.BaseType;
            while (parent != null && typeof(IMessage).IsAssignableFrom(parent))
            {
                //ToDo: rewrire
                var parentInfo = concreteTypeToHandlers.GetOrNull(parent);
                var infoWasCached = parentInfo != null;
                if (!infoWasCached)
                {
                    concreteTypeToHandlers[parent] = parentInfo = new ConcreteTypeHandlers(parent);
                    parentClassesAdded.Add(parentInfo);
                }
                else if (firstParentInCache == null)
                {
                    firstParentInCache = parentInfo;
                }

                parentInfo.ExpandedTypes.AddRange(intermediateTypes);
                parentInfo.ExpandedTypes.Add(type);

                //If we were not in the msgHandlerCache, this type is newly created and must be added to parents
                if (!infoWasCached)
                {
                    intermediateTypes.Add(parent);
                }

                parent = parent.BaseType;
            }

            //Add inherited listeners to newly cached types
            if (firstParentInCache != null)
            {

                foreach (var handler in firstParentInCache.Handlers.Keys)
                {
                    foreach (var parentInfo in parentClassesAdded)
                    {
                        AddHandlerToType(parentInfo, handler);
                    }

                    AddHandlerToType(info, handler);
                }
            }

        }
    }
}