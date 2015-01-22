using Amazon.SQS;

namespace JUSTEAT.Amazon.ServiceBus.Publishers
{
    public class SQSPublisher : IPublisher
    {
        private readonly IAmazonSQS _amazonSQSClient;
        private readonly QueueConfiguration _queueConfiguration;
        private bool _configured;
        public string QueueUrl { get; private set; }

        public SQSPublisher(IAmazonSQS amazonSQSClient, string queueUrl) :
            this(amazonSQSClient, new QueueConfiguration(queueUrl))
        {
        }

        public SQSPublisher(IAmazonSQS amazonSQSClient, QueueByName queueByName) :
            this(amazonSQSClient, new QueueConfiguration(amazonSQSClient, queueByName))
        {
        }

        public SQSPublisher(IAmazonSQS amazonSQSClient, QueueConfiguration queueConfiguration)
        {
            _amazonSQSClient = amazonSQSClient;
            _queueConfiguration = queueConfiguration;
        }

        public void Configure()
        {
            if (_configured)
            {
                return;
            }

            QueueUrl = _queueConfiguration.GetQueueUrl();

            _configured = true;
        }

        public void Publish(string message)
        {
            if (!_configured)
            {
                Configure();
            }

            _amazonSQSClient.SendMessage(QueueUrl, message);
        }
    }
}