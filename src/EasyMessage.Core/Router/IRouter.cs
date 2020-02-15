using System.Collections.Generic;
using EasyMessage.Core.Messages;

namespace EasyMessage.Core.Router
{
    /// <summary>
    /// Routes messages to registered handlers.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Registers an instance to its declared IHandleMessage types
        /// </summary>
        /// <param name="instance"></param>
        void Register(object instance);

        /// <summary>
        /// UnRegister an instance to its declared IHandleMessage types
        /// </summary>
        /// <param name="instance"></param>
        void UnRegister(object instance);

        /// <summary>
        /// Gets handlers for message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IEnumerable<TaggedAction<IMessage>> GetHandlers(IMessage message);
    }
}