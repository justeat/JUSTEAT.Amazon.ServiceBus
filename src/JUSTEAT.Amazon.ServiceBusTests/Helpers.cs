using System;
using System.Linq.Expressions;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;

namespace JUSTEAT.Amazon.ServiceBusTests
{
    public class Helpers
    {
        public static bool GetQueueUrlRquestMatches(GetQueueUrlRequest request, string expectedQueueName)
        {
            return request.QueueName.Equals(expectedQueueName);
        }

        public static bool CreateQueueRequestMatches(CreateQueueRequest request, string expectedQueueName)
        {
            return request.QueueName.Equals(expectedQueueName);
        }

        public static bool ReceiveMessageRequestMatches(ReceiveMessageRequest request, string expectedQueueUrl, int expectedMaxNumberOfMessages)
        {
            return request.QueueUrl.Equals(expectedQueueUrl)
                   & request.MaxNumberOfMessages.Equals(expectedMaxNumberOfMessages);
        }

        public static ReceiveMessageResponse ReceiveMessageResponse(string message)
        {
            return new ReceiveMessageResponse { Messages = new System.Collections.Generic.List<Message> { new Message { Body = message } } };
        }

        public static Expression<Func<IAmazonSQS, ReceiveMessageResponse>> SetupReceiveMessage(string expectedQueueUrl, int expectedMaxNumberOfMessages)
        {
            return x => x.ReceiveMessage(It.Is<ReceiveMessageRequest>(y => ReceiveMessageRequestMatches(y, expectedQueueUrl, expectedMaxNumberOfMessages)));
        }
    }
}