namespace JUSTEAT.Amazon.ServiceBus
{
    public class QueueByName
    {
        public QueueByName(string queueName, bool createQueue)
        {
            QueueName = queueName;
            CreateQueue = createQueue;
        }

        public string QueueName { get; set; }
        public bool CreateQueue { get; set; }
    }
}