using System;
using System.Collections.Generic;
using EasyMessage.Core.Messages;
using EasyMessage.Core.Router;

namespace EasyMessage.Core
{
    public class MessageDispatcher : IDispatcher
    {
        private readonly IRouter router;
        private readonly Guid serverId;

        public MessageDispatcher(IRouter router, Guid serverId)
        {
            this.router = router;
            this.serverId = serverId;
        }

        /// <inheritdoc />
        public void DispatchIncomeMessage(IMessageEnvelope messageEnvelope)
        {
            if (messageEnvelope?.Header == null || messageEnvelope.Message == null)
            {
                return;
            }

            if (messageEnvelope.Header.SenderId == serverId)
            {
                return;
            }

            var handlers = FindExternalHandlers(messageEnvelope.Message);

            Dispatch(messageEnvelope.Message, handlers);
            
        }

        private void Dispatch(IMessage message, IEnumerable<TaggedAction<IMessage>> handlers)
        {
            foreach (var handler in handlers)
            {
                try
                {
                    handler.Action(message);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

        }

        private IEnumerable<TaggedAction<IMessage>> FindExternalHandlers(IMessage message)
        {
            return message == null ? null : router.GetHandlers(message);
        }
    }
}