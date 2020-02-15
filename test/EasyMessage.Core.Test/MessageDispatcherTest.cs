using System;
using Moq;
using NUnit.Framework;
using EasyMessage.Core.Messages;
using EasyMessage.Core.Router;


namespace EasyMessage.Core.Test
{
    [TestFixture]
    public class MessageDispatcherTest
    {
        private readonly Mock<IRouter> routerMock = new Mock<IRouter>();
        private readonly Guid server1 = Guid.NewGuid();
        private readonly Guid server2 = Guid.NewGuid();

        private MessageDispatcher messageDispatcherServer1;
        private MessageDispatcher messageDispatcherServer2;
        private MessageEnveloperFactory messageEnveloperFactoryServer1;
        private MessageEnveloperFactory messageEnveloperFactoryServer2;

        [OneTimeSetUp]
        public void Setup()
        {
            messageDispatcherServer1 = new MessageDispatcher(routerMock.Object, server1);
            messageEnveloperFactoryServer1 = new MessageEnveloperFactory(server1);

            messageDispatcherServer2 = new MessageDispatcher(routerMock.Object, server2);
            messageEnveloperFactoryServer2 = new MessageEnveloperFactory(server2);
        }

        [Test]
        public void HandleEnvelopeFromServer1OnServer1_NeverGetHandlers()
        {
            // Arrange
            routerMock.Invocations.Clear();
            var envelopeFromServer1 = messageEnveloperFactoryServer1.Stuff(new TestMessage());

            // Act
            messageDispatcherServer1.DispatchIncomeMessage(envelopeFromServer1);

            // Assert
            routerMock.Verify(x => x.GetHandlers(It.IsAny<IMessage>()), Times.Never);
        }

        [Test]
        public void HandleEnvelopeFromServer2OnServer2_NeverGetHandlers()
        {
            // Arrange
            routerMock.Invocations.Clear();
            var envelopeFromServer2 = messageEnveloperFactoryServer2.Stuff(new TestMessage());

            // Act
            messageDispatcherServer2.DispatchIncomeMessage(envelopeFromServer2);

            // Assert
            routerMock.Verify(x => x.GetHandlers(It.IsAny<IMessage>()), Times.Never);
        }

        [Test]
        public void HandleEnvelopeWithOutMessage_NeverGetHandlers()
        {
            // Arrange
            routerMock.Invocations.Clear();
            var envelopeFromServer1 = new MessageEnvelope(new MessageHeader(server1, null), null);
            var envelopeFromServer2 = new MessageEnvelope(new MessageHeader(server2, null), null);

            // Act
            messageDispatcherServer1.DispatchIncomeMessage(envelopeFromServer1);
            messageDispatcherServer2.DispatchIncomeMessage(envelopeFromServer2);

            // Assert
            routerMock.Verify(x => x.GetHandlers(It.IsAny<IMessage>()), Times.Never);
        }

        [Test]
        public void HandleEnvelopeFromServer2OnSever1_GetHandlersOnce()
        {
            // Arrange
            routerMock.Invocations.Clear();
            var envelopeFromServer2 = messageEnveloperFactoryServer2.Stuff(new TestMessage());

            // Act
            messageDispatcherServer1.DispatchIncomeMessage(envelopeFromServer2);

            // Assert
            routerMock.Verify(x => x.GetHandlers(It.IsAny<IMessage>()), Times.Once);
        }

        [Test]
        public void HandleEnvelopeFromServer1OnSever2_GetHandlersOnce()
        {
            // Arrange
            routerMock.Invocations.Clear();
            var envelopeFromServer1 = messageEnveloperFactoryServer1.Stuff(new TestMessage());

            // Act
            messageDispatcherServer2.DispatchIncomeMessage(envelopeFromServer1);

            // Assert
            routerMock.Verify(x => x.GetHandlers(It.IsAny<IMessage>()), Times.Once);
        }
    }
}