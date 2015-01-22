using System;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using NUnit.Framework;
using Should;

namespace JUSTEAT.Amazon.ServiceBusTests.Receivers.SQSReceiverTests
{
    public class SQSReciever_MessagesHandlerThrowsException : SQSRecieverTestBase
    {
        private bool _onErrorWasCalled;
        private Exception _onErrorExceptionCaught;

        [Test]
        public void AmazonSQSClient_DeletedMessage_IsNeverCalled()
        {
            GetMockFor<IAmazonSQS>()
                .Verify(x => x.DeleteMessage(It.IsAny<DeleteMessageRequest>()), Times.Never);
        }

        [Test]
        public void AmazonSQSClient_OnError_WasCalled()
        {
            _onErrorWasCalled.ShouldBeTrue();
        }

        [Test]
        public void AmazonSQSClient_OnError_ExceptionIsTypeOfNotSupportedException()
        {
            _onErrorExceptionCaught.ShouldBeType<NotSupportedException>();
        }

        protected override bool OnMessageRecieved(Message message)
        {
            throw new NotSupportedException();
        }

        protected override void OnError(Exception ex, Message message)
        {
            _onErrorWasCalled = true;
            _onErrorExceptionCaught = ex;
        }
    }
}