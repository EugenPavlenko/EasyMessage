using System;

namespace EasyMessage.Core.Messages
{
    [Serializable]
    public class MessageEnvelope : IMessageEnvelope
    {
        /// <inheritdoc cref="IMessageEnvelope.Header"/>
        public IMessageHeader Header { get; set; }
        
        /// <inheritdoc cref="IMessageEnvelope.Message"/>
        public IMessage Message { get; set; }

        public MessageEnvelope(IMessageHeader header, IMessage message)
        {
            Header = header;
            Message = message;
        }
    }
}