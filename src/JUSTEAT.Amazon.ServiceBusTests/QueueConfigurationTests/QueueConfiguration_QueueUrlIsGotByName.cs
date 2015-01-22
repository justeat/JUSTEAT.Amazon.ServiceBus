using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus;
using Moq;
using NUnit.Framework;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.QueueConfigurationTests
{
    public class QueueConfiguration_QueueUrlIsGotByName : SpecsFor<QueueConfiguration>
    {
        private const string QueueUrl = "http://my-queue";
        private const string QueueName = "my-queue";

        protected override void InitializeClassUnderTest()
        {
            SUT = new QueueConfiguration(GetMockFor<IAmazonSQS>().Object, new QueueByName(QueueName, false));
        }

        protected override void Given()
        {
            GetMockFor<IAmazonSQS>()
                .Setup(x => x.GetQueueUrl(It.Is<GetQueueUrlRequest>(y => Helpers.GetQueueUrlRquestMatches(y, QueueName))))
                .Returns(new GetQueueUrlResponse{ QueueUrl = QueueUrl});
        }

        protected override void When()
        {
            SUT.GetQueueUrl();
        }

        [Test]
        public void AmazonSQS_CreateQueue_IsNotRecieved_EnsuringQueueWasNotCreated()
        {
            GetMockFor<IAmazonSQS>().Verify(
                x => x.CreateQueue(It.IsAny<CreateQueueRequest>()), Times.Never);
        }
    }
}