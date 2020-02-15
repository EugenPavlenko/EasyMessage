using System;
using System.Linq;
using System.Runtime.InteropServices;
using EasyMessage.Core.Messages;
using EasyMessage.Core.Router;
using Moq;
using NUnit.Framework;

namespace EasyMessage.Core.Test
{
    [TestFixture]
    public class MessageRouterTest
    {
        [Test]
        public void RegisterForNullHandler_ArgumentException()
        {
            var messageRouter = new MessageRouter();
            Assert.Throws(typeof(ArgumentNullException), () => messageRouter.Register(null));
        }

        [Test]
        public void RegisterNoIHandlerInstance_ArgumentException()
        {
            var messageRouter = new MessageRouter();
            Assert.Throws(typeof(ArgumentException), () => messageRouter.Register(new object()));
        }

        [Test]
        public void RegisterHandlerForTestMessage_NoException()
        {
            var messageRouter = new MessageRouter();
            var handler = Mock.Of<IHandleMessage<IMessage>>();
            messageRouter.Register(handler);
        }

        [Test]
        public void RegisterOneHandlerForTestMessage_OneHandler()
        {
            // arrange
            var messageRouter = new MessageRouter();
            var handler = Mock.Of<IHandleMessage<TestMessage>>();

            // act
            messageRouter.Register(handler);

            // assert
            Assert.AreEqual(1, messageRouter.GetHandlers(new TestMessage()).Count());
        }

        [Test]
        public void RegisterTwoHandlerForTestMessage_TwoHandler()
        {
            // arrange
            var messageRouter = new MessageRouter();
            var handler1 = new TestMessageHandler1();
            var handler2 = new TestMessageHandler2();

            // act
            messageRouter.Register(handler1);
            messageRouter.Register(handler2);

            // assert
            Assert.AreEqual(2, messageRouter.GetHandlers(new TestMessage()).Count());
        }

        [Test]
        public void RegisterOneHandlerForTestMessageAndUnRegisterOne_ZeroHandler()
        {
            // arrange
            var messageRouter = new MessageRouter();
            var handler1 = new TestMessageHandler1();

            // act
            messageRouter.Register(handler1);
            messageRouter.UnRegister(handler1);

            // assert
            Assert.AreEqual(0, messageRouter.GetHandlers(new TestMessage()).Count());
        }

        [Test]
        public void RegisterTwoHandlerForTestMessageAndUnRegisterOne_OneHandler()
        {
            // arrange
            var messageRouter = new MessageRouter();
            var handler1 = new TestMessageHandler1();
            var handler2 = new TestMessageHandler2();

            // act
            messageRouter.Register(handler1);
            messageRouter.Register(handler2);
            messageRouter.UnRegister(handler1);
            
            // assert
            Assert.AreEqual(1, messageRouter.GetHandlers(new TestMessage()).Count());
        }
    }
}