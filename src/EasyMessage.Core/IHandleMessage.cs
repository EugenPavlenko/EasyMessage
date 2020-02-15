using EasyMessage.Core.Messages;

namespace EasyMessage.Core
{
    /// <summary>
    /// Handles  messages.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IHandleMessage<in TMessage> where TMessage : IMessage
    {
        /// <summary>
        /// Handles should be quick, or delegate to a thread pool.
        /// </summary>
        /// <param name="message">Message to handle</param>
        void HandleMessage(TMessage message);
    }
}