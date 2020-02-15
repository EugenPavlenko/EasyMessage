using System;

namespace EasyMessage.Core.Messages
{
    [Serializable]
    public class MessageHeader : IMessageHeader
    {
        public MessageHeader(Guid senderId, string messageId)
        {
            SenderId = senderId;
            MessageId = messageId;
        }

        /// <inheritdoc cref="IMessageHeader.SenderId"/>
        public Guid SenderId { get; set; }

        /// <inheritdoc cref="IMessageHeader.MessageId"/>
        public string MessageId { get; set; }
    }
}