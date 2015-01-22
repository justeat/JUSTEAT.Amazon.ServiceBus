using System;
using System.Threading;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using JUSTEAT.Amazon.ServiceBus.Receivers;
using Moq;
using NUnit.Framework;
using SpecsFor;
using Should;

namespace JUSTEAT.Amazon.ServiceBusTests.SNSToSQSServiceBusTests
{
    public class SNSToSQSServiceBus_CoordinatesPublisherAndReceiver : SpecsFor<SNSToSQSServiceBus>
    {
        private const string TopicArn = "arn:my-topic";
        private const string QueueUrl = "http://my-queue";
        private const string Message = "my-message";
        private Message _actualRecievedMessage;

        protected override void InitializeClassUnderTest()
        {
            var publiser = new SNSPublisher(GetMockFor<IAmazonSimpleNotificationService>().Object,TopicArn);
            var receiver = new SequentialSQSReceiver(GetMockFor<IAmazonSQS>().Object, QueueUrl, message =>
            {
                _actualRecievedMessage = message;
                return true;
            }, null);

            SUT = new SNSToSQSServiceBus(publiser, receiver);
        }

        
        protected override void Given()
        {
            GetMockFor<IAmazonSimpleNotificationService>()
                .Setup(x => x.Publish(TopicArn, Message))
                .Returns(new PublishResponse());

            GetMockFor<IAmazonSQS>()
                .Setup(Helpers.SetupReceiveMessage(QueueUrl, 1))
                .Returns(Helpers.ReceiveMessageResponse(Message));

        }

        protected override void When()
        {
            SUT.StartReceiving();
            SUT.Publish(Message);
            SpinWait.SpinUntil(() => _actualRecievedMessage != null, TimeSpan.FromSeconds(5));
        }

        [Test]
        public void AmazonSNS_Pubish_IsRecieved_IndicatingMessageWasPublished()
        {
            GetMockFor<IAmazonSimpleNotificationService>().Verify(
                x => x.Publish(TopicArn, Message));
        }

        [Test]
        public void AmazonSQS_ReceiveMessage_IsRecieved_IndicatingMessagesWerebeingReceived()
        {
            GetMockFor<IAmazonSQS>().Verify(
                x => x.ReceiveMessage(It.Is<ReceiveMessageRequest>(y => Helpers.ReceiveMessageRequestMatches(y, QueueUrl, 1))));
        }

        [Test]
        public void ActualMessageRecieved_IsNotNull_IndicatingReceiverWasReceivingMessages()
        {
            _actualRecievedMessage.ShouldNotBeNull();
        }

        [Test]
        public void ActualMessageRecieved_MessageIsEqualToExpected_IndicatingReceiverWasReceivingTheExpectedMessage()
        {
            _actualRecievedMessage.Body.ShouldEqual(Message);
        }
    }
}