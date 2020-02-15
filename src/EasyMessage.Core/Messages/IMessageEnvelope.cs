namespace EasyMessage.Core.Messages
{
    public interface IMessageEnvelope
    {
        /// <summary>
        /// Envelop header
        /// </summary>
        IMessageHeader Header { get; }

        /// <summary>
        /// Message in envelope
        /// </summary>
        IMessage Message { get; }
    }
}