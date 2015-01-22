using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace JUSTEAT.Amazon.ServiceBus.Publishers
{
    public class SNSPublisher : IPublisher
    {
        private readonly IAmazonSimpleNotificationService _amazonSNSClient;
        private readonly TopicConfiguration _topicConfiguration;
        private bool _configured;
        public string TopicArn { get; private set; }

        public SNSPublisher(IAmazonSimpleNotificationService amazonSNSClient, string topicArn) :
            this(amazonSNSClient, new TopicConfiguration(topicArn))
        {
        }

        public SNSPublisher(IAmazonSimpleNotificationService amazonSNSClient, TopicByName toicByName) :
            this(amazonSNSClient, new TopicConfiguration(amazonSNSClient, toicByName))
        {
        }

        public SNSPublisher(IAmazonSimpleNotificationService amazonSNSClient, TopicConfiguration topicConfiguration)
        {
            _amazonSNSClient = amazonSNSClient;
            _topicConfiguration = topicConfiguration;
        }

        public void Configure()
        {
            if (_configured)
            {
                return;
            }

            TopicArn = _topicConfiguration.GetTopicArn();

            _configured = true;
        }

        public void Publish(string message)
        {
            if (!_configured)
            {
                Configure();
            }

            _amazonSNSClient.Publish(TopicArn, message);
        }


        public void Subscribe(IAmazonSQS amazonSqsClient, string queueUrl)
        {
            if (!_configured)
            {
                Configure();
            }

            _amazonSNSClient.SubscribeQueue(TopicArn, amazonSqsClient, queueUrl);
        }
    }
}