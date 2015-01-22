using Amazon.SimpleNotificationService;

namespace JUSTEAT.Amazon.ServiceBus
{
    public class TopicConfiguration
    {
        private readonly TopicByName _topicByName;
        private readonly IAmazonSimpleNotificationService _amazonSNSClient;
        private readonly string _topicArn;

        public TopicConfiguration(string topicArn)
        {
            _topicArn = topicArn;
        }

        public TopicConfiguration(IAmazonSimpleNotificationService amazonSNSClient, TopicByName topicByName)
        {
            _amazonSNSClient = amazonSNSClient;
            _topicByName = topicByName;
        }

        public string GetTopicArn()
        {
            if (!string.IsNullOrEmpty(_topicArn))
            {
                return _topicArn;
            }

            if (_topicByName.CreateTopic)
            {
                var createTopicResponse = _amazonSNSClient.CreateTopic(_topicByName.TopicName);
                return createTopicResponse.TopicArn;
            }

            return _amazonSNSClient.FindTopic(_topicByName.TopicName).TopicArn;
        }
    }
}