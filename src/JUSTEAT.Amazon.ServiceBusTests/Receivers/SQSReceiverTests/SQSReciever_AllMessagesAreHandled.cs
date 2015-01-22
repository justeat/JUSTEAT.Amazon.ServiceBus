using System.Collections.Generic;
using Amazon.SQS.Model;
using NUnit.Framework;
using Should;

namespace JUSTEAT.Amazon.ServiceBusTests.Receivers.SQSReceiverTests
{
    public class SQSReciever_AllMessagesAreHandled : SQSRecieverTestBase
    {
        private readonly List<Message> _messagesReceived = new List<Message>();

        public SQSReciever_AllMessagesAreHandled()
        {
            TotalNumberOfMessages = 20;
            MaxNumberOfMessages = 10;
        }

        [Test]
        public void MessagesReceived_Is20()
        {
            _messagesReceived.Count.ShouldEqual(20);
        }

        protected override bool OnMessageRecieved(Message message)
        {
            _messagesReceived.Add(message);
            return true;
        }


    }
}