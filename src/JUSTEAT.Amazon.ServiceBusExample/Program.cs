using System;
using System.Threading;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;
using JUSTEAT.Amazon.ServiceBus;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using JUSTEAT.Amazon.ServiceBus.Receivers;

namespace JUSTEAT.Amazon.ServiceBusExample
{
    public class Program
    {
        private static bool _completed;
        private static bool _completedSuccessfully;

        public static void Main(string[] args)
        {
            var receiver = new ConcurrentSequentialSqsReceiver(
                new AmazonSQSClient(), 
                new QueueByName("justeat-servicebus-example-queue", true),
                ProcessMessage, 
                ProcessError);

            var publisher = new SNSPublisher(
                new AmazonSimpleNotificationServiceClient(),
                new TopicByName("justeat-servicebus-example-topic", true));

            //var publisher = new SQSPublisher(
            //    new AmazonSQSClient(),
            //    new QueueByName("justeat-servicebus-example-queue", true));

            var serviceBus = new SNSToSQSServiceBus(publisher, receiver);
            serviceBus.StartReceiving();

            var message = string.Format(@"{{""id"":{0}}}", new Random().Next());
            
            Console.WriteLine("Publishing Message: {0}", message);

            serviceBus.Publish(message);

            SpinWait.SpinUntil(() => _completed, TimeSpan.FromSeconds(10));

            if (_completed)
            {
                if (_completedSuccessfully)
                {
                    Console.WriteLine("Message recieved and handled successfuly");
                }
                else
                {
                    Console.WriteLine("Message recieved but an unexpected error occured.  Please submit a github issue.");
                }

            }
            else
            {
                Console.WriteLine("Something has unexpectedly gone wrong. Please submit a github issue and try " +
                                  "enabling logging in the AWS SDK to capture useful output: " +
                                  "http://docs.aws.amazon.com/AWSSdkDocsNET/latest/DeveloperGuide/net-dg-config-other.html#config-setting-awslogging");
            }

            serviceBus.StopReceiving();

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        public static bool ProcessMessage(Message message)
        {
            Console.WriteLine("Message Recieved: {0}", message.Body);
            _completed = true;
            _completedSuccessfully = true;
            return true;
        }

        public static void ProcessError(Exception ex, Message message)
        {
            Console.WriteLine("Error: {0}, Message Body: {0}", ex, message.Body);
            _completed = true;
        }
    }
}
