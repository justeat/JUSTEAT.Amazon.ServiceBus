using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using NUnit.Framework;
using Should;

namespace JUSTEAT.Amazon.ServiceBusTests.Receivers.SQSReceiverTests
{
    public class SQSReciever_MessagesAreHandldedSuccessfully : SQSRecieverTestBase
    {
        private readonly List<Message> _messagesReceived = new List<Message>();

        public SQSReciever_MessagesAreHandldedSuccessfully()
        {
            MaxNumberOfMessages = 1;
        }

        [Test]
        public void MessagesReceived_Is1()
        {
            _messagesReceived.Count.ShouldEqual(1);
        }

        [Test]
        public void AmazonSQSClient_DeletedMessage_IsCalled()
        {
            GetMockFor<IAmazonSQS>()
                .Verify(x => x.DeleteMessage(
                    It.Is<DeleteMessageRequest>(
                        y => _messagesReceived.Any(z => z.ReceiptHandle.Equals(y.ReceiptHandle))
                    )),
                    String.Join(",", _messagesReceived.Select(x => x.ReceiptHandle)));
        }

        protected override bool OnMessageRecieved(Message message)
        {
            _messagesReceived.Add(message);
            return true;
        }
    }
}
