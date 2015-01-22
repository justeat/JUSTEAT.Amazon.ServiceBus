using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using NUnit.Framework;

namespace JUSTEAT.Amazon.ServiceBusTests.Receivers.SQSReceiverTests
{
    public class SQSReciever_MessagesAreHandldedUnsuccessfully : SQSRecieverTestBase
    {
        [Test]
        public void AmazonSQSClient_DeletedMessage_IsNeverCalled()
        {
            GetMockFor<IAmazonSQS>()
                .Verify(x => x.DeleteMessage(It.IsAny<DeleteMessageRequest>()), Times.Never);
        }

        protected override bool OnMessageRecieved(Message message)
        {
            return false;
        }
    }
}