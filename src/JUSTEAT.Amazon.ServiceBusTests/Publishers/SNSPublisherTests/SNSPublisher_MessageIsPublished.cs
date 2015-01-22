using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.Publishers.SNSPublisherTests
{
    public class SNSPublisher_MessageIsPublished : SpecsFor<SNSPublisher>
    {
        private const string TopicArn = "arn:topic";
        private const string Message = "unit-test-message";


        protected override void InitializeClassUnderTest()
        {
            SUT = new SNSPublisher(GetMockFor<IAmazonSimpleNotificationService>().Object, TopicArn);
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSimpleNotificationService>()
                .Setup(x => x.Publish(TopicArn, Message))
                .Returns(() => new PublishResponse());
        }

        protected override void When()
        {
            SUT.Publish(Message);
        }

        [Test]
        public void AmazonSQS_SendMessage_IsRecieved_IndicatingMessageWasPublished()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.Publish(TopicArn, Message));
        }
    }
}