using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using JUSTEAT.Amazon.ServiceBus;
using Moq;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.TopicConfigurationTests
{
    public class TopicConfiguration_TopicIsFoundByName : SpecsFor<TopicConfiguration>
    {
        private const string TopicArn = "arn:my-topic";
        private const string TopicName = "my-topic";

        protected override void InitializeClassUnderTest()
        {
            SUT = new TopicConfiguration(
                GetMockFor<IAmazonSimpleNotificationService>().Object, 
                new TopicByName(TopicName, false));
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSimpleNotificationService>()
                .Setup(x => x.FindTopic(TopicName))
                .Returns(new Topic { TopicArn = TopicArn});
        }

        protected override void When()
        {
            SUT.GetTopicArn();
        }

        [Test]
        public void AmazonSNS_CreateTopic_IsNotRecieved_EnsuringTopicWasNotCreated()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.CreateTopic(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void AmazonSNS_FinTodpic_IsRecieved_IndicatingTopicWasFoundByTopicName()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.FindTopic(TopicName));
        }
    }
}