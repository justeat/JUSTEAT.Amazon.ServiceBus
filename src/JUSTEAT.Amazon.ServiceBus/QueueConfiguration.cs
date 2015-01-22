using Amazon.SQS;
using Amazon.SQS.Model;

namespace JUSTEAT.Amazon.ServiceBus
{
    public class QueueConfiguration
    {
        private readonly QueueByName _queueByName;
        private readonly IAmazonSQS _amazonSQSClient;
        private readonly string _queueUrl;

        public QueueConfiguration(string queueUrl)
        {
            _queueUrl = queueUrl;
        }

        public QueueConfiguration(IAmazonSQS amazonSqsClient, QueueByName queueByName)
        {
            _amazonSQSClient = amazonSqsClient;
            _queueByName = queueByName;
        }

        public string GetQueueUrl()
        {
            if (!string.IsNullOrEmpty(_queueUrl))
            {
                return _queueUrl;
            }

            if (_queueByName.CreateQueue)
            {
                var createQueueResponse = _amazonSQSClient.CreateQueue(new CreateQueueRequest(_queueByName.QueueName));
                return createQueueResponse.QueueUrl;
            }

            var getQueueUrlResponse = _amazonSQSClient.GetQueueUrl(new GetQueueUrlRequest(_queueByName.QueueName));
            return getQueueUrlResponse.QueueUrl;
        }
        
    }
}