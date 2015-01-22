using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using JUSTEAT.Amazon.ServiceBus;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.TopicConfigurationTests
{
    public class TopicConfiguration_TopicIsCreated : SpecsFor<TopicConfiguration>
    {
        private const string TopicArn = "arn:my-topic";
        private const string TopicName = "my-topic";

        protected override void InitializeClassUnderTest()
        {
            SUT = new TopicConfiguration(
                GetMockFor<IAmazonSimpleNotificationService>().Object, 
                new TopicByName(TopicName, true));
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSimpleNotificationService>()
                .Setup(x => x.CreateTopic(TopicName))
                .Returns(new CreateTopicResponse{ TopicArn = TopicArn});
        }

        protected override void When()
        {
            SUT.GetTopicArn();
        }

        [Test]
        public void AmazonSNS_CreateTopic_IsRecieved_IndicatingTopicWasCreated()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.CreateTopic(TopicName));
        }
    }
}