using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus;
using Moq;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.QueueConfigurationTests
{
    public class QueueConfiguration_QueueIsCreated : SpecsFor<QueueConfiguration>
    {
        private const string QueueUrl = "http://my-queue";
        private const string QueueName = "my-queue";

        protected override void InitializeClassUnderTest()
        {
            SUT = new QueueConfiguration(GetMockFor<IAmazonSQS>().Object, new QueueByName(QueueName, true));
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSQS>()
                .Setup(x => x.CreateQueue(It.Is<CreateQueueRequest>(y => Helpers.CreateQueueRequestMatches(y, QueueName))))
                .Returns(new CreateQueueResponse{ QueueUrl = QueueUrl});
        }

        protected override void When()
        {
            SUT.GetQueueUrl();
        }

        [Test]
        public void AmazonSQS_CreateQueue_IsRecieved_IndicatingQueueWasCreated()
        {
            GetMockFor<IAmazonSQS>().Verify(
                x => x.CreateQueue(It.Is<CreateQueueRequest>(y => Helpers.CreateQueueRequestMatches(y, QueueName))));
        }
    }
}