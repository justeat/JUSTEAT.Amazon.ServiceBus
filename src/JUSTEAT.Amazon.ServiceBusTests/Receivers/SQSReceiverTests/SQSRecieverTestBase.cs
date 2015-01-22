using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus;
using JUSTEAT.Amazon.ServiceBus.Receivers;
using Moq;
using SpecsFor;

namespace JUSTEAT.Amazon.ServiceBusTests.Receivers.SQSReceiverTests
{
    public class SQSRecieverTestBase : SpecsFor<SequentialSQSReceiver>
    {
        private const string QueueUrl = "http://my-queue";
        private List<Message> _messages;

        protected int MaxNumberOfMessages;
        protected int TotalNumberOfMessages;
        private Timer _stopProcessingTimer;

        public SQSRecieverTestBase()
        {
            TotalNumberOfMessages = 1;
            MaxNumberOfMessages = 1;
        }

        protected override void InitializeClassUnderTest()
        {
            SUT = new SequentialSQSReceiver(GetMockFor<IAmazonSQS>().Object, QueueUrl, OnMessageRecieved, OnError, MaxNumberOfMessages);
        }

        protected override void Given()
        {
            _messages = CreateMessages(TotalNumberOfMessages);

            var timesToIterate = Math.Ceiling(TotalNumberOfMessages/(decimal)MaxNumberOfMessages);
            var iterations = 0;

            GetMockFor<IAmazonSQS>()
                .Setup(x => x.ReceiveMessage(
                    It.Is<ReceiveMessageRequest>(y => Helpers.ReceiveMessageRequestMatches(y, QueueUrl, MaxNumberOfMessages))))
                .Returns(() =>
                {
                    ReceiveMessageResponse response;

                    if (iterations < timesToIterate)
                    {
                        response = new ReceiveMessageResponse
                        {
                            Messages = _messages.Skip(iterations).Take(MaxNumberOfMessages).ToList()
                        };

                    }
                    else
                    {
                        response = new ReceiveMessageResponse
                        {
                            Messages = new List<Message>()
                        };
                    }

                    iterations++;
                    return response;
                });
        }

        protected override void When()
        {
            var t = new Task(SUT.StartReceiving);
            t.Start();
            SpinWait.SpinUntil(() => SUT.IsReceiving, TimeSpan.FromSeconds(5));
            _stopProcessingTimer = new Timer(StopProcessingTimerCallback, null, TimeSpan.FromSeconds(5),
                Timeout.InfiniteTimeSpan);
        }

        protected virtual bool OnMessageRecieved(Message message)
        {
            return true;
        }

        protected virtual void OnError(Exception ex, Message message)
        {
            
        }

        private void StopProcessingTimerCallback(object obj)
        {
            SUT.StopReceiving();
        }

        private static List<Message> CreateMessages(int number)
        {
            return Enumerable.Range(0, number)
                .Select(x => new Message
                {
                    ReceiptHandle = number.ToString(CultureInfo.InvariantCulture)
                })
                .ToList();
        }
    }
}