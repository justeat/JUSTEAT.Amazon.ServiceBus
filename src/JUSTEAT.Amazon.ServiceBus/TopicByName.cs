namespace JUSTEAT.Amazon.ServiceBus
{
    public class TopicByName
    {
        public TopicByName(string topicName, bool createTopic)
        {
            TopicName = topicName;
            CreateTopic = createTopic;
        }

        public string TopicName { get; set; }
        public bool CreateTopic { get; set; }
    }
}