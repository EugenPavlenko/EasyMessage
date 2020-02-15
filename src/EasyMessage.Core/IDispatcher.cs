using EasyMessage.Core.Messages;

namespace EasyMessage.Core
{
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatch incoming message.
        /// </summary>
        /// <param name="messageEnvelope">Income message Envelope</param>
        void DispatchIncomeMessage(IMessageEnvelope messageEnvelope);
    }
}