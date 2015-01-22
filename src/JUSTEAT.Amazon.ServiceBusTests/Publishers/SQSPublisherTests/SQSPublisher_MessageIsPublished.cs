using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.Publishers.SQSPublisherTests
{
    public class SQSPublisher_MessageIsPublished : SpecsFor<SQSPublisher>
    {
        private const string QueueUrl = "http://my-queue";
        private const string Message = "unit-test-message";


        protected override void InitializeClassUnderTest()
        {
            SUT = new SQSPublisher(GetMockFor<IAmazonSQS>().Object, QueueUrl);
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSQS>()
                .Setup(x => x.SendMessage(QueueUrl, Message))
                .Returns(() => new SendMessageResponse());
        }

        protected override void When()
        {
            SUT.Publish(Message);
        }

        [Test]
        public void AmazonSQS_SendMessage_IsRecieved_IndicatingMessageWasPublished()
        {
            GetMockFor<IAmazonSQS>().Verify(
                x => x.SendMessage(QueueUrl, Message));
        }
    }
}