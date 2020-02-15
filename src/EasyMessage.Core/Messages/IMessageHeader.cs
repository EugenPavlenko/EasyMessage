using System;

namespace EasyMessage.Core.Messages
{
    public interface IMessageHeader
    {
        /// <summary>
        /// Server Run ID of the message sender
        /// </summary>
        Guid SenderId { get; set; }

        /// <summary>
        /// Unique identifier (per server run) of the message
        /// </summary>
        string MessageId { get; set; }
    }
}