using System;

namespace EasyMessage.Core.Messages
{
    public class MessageEnveloperFactory : IMessageEnveloper
    {
        private readonly Guid serverId;

        public MessageEnveloperFactory(Guid serverId)
        {
            this.serverId = serverId;
        }

        /// <inheritdoc cref="IMessageEnveloper.Stuff"/>
        public IMessageEnvelope Stuff(IMessage message)
        {
            var header = CreateHeader(message);
            return new MessageEnvelope(header, message);
        }

        private IMessageHeader CreateHeader(IMessage message)
        {
            var messageId = GenerateNextMessageId(message);
            return new MessageHeader(serverId, messageId);
        }

        private static string GenerateNextMessageId(IMessage message)
        {
            return $"{Guid.NewGuid()}-{message.GetHashCode()}";
        }
    }
}