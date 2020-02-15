using System.Collections.Generic;
using EasyMessage.Core.Messages;

namespace EasyMessage.Core.Router
{
    internal class MessageActions
    {
        public IEnumerable<TaggedAction<IMessage>> Actions { get; }

        public MessageActions(IEnumerable<TaggedAction<IMessage>> actions)
        {
            Actions = actions;
        }

    }
}