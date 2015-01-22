using Amazon.SimpleNotificationService;
using Amazon.SQS;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.Publishers.SNSPublisherTests
{
    public class SNSPublisher_QueueIsSubscribed : SpecsFor<SNSPublisher>
    {
        private const string TopicArn = "arn:topic";
        private const string QueueUrl = "http://my-queue";


        protected override void InitializeClassUnderTest()
        {
            SUT = new SNSPublisher(GetMockFor<IAmazonSimpleNotificationService>().Object, TopicArn);
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSimpleNotificationService>()
                .Setup(x => x.SubscribeQueue(TopicArn, GetMockFor<IAmazonSQS>().Object, QueueUrl))
                .Returns(() => "arn:subscription");
        }

        protected override void When()
        {
            SUT.Subscribe(GetMockFor<IAmazonSQS>().Object, QueueUrl);
        }

        [Test]
        public void AmazonSimpleNotificationService_SubscribeQueue_IsRecieved_IndicatingQueueWasSubscribedToTopic()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.SubscribeQueue(TopicArn, GetMockFor<IAmazonSQS>().Object, QueueUrl));
        }
    }
}