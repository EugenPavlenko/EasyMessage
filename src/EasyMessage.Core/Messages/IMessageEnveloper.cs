namespace EasyMessage.Core.Messages
{
    public interface IMessageEnveloper
    {
        /// <summary>
        /// Wraps a IMessage into a IMessageEnvelope, populating header information.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IMessageEnvelope Stuff(IMessage message);
    }
}