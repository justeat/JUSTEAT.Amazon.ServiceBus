using System.Threading;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using JUSTEAT.Amazon.ServiceBus.Receivers;

namespace JUSTEAT.Amazon.ServiceBus
{
    public class SNSToSQSServiceBus : IServiceBus
    {
        private readonly SNSPublisher _publisher;
        private readonly SequentialSQSReceiver _receiver;
        private readonly Thread _sQSReceiverThread;

        private bool _configured;
        private readonly bool _createSubscription;

        public SNSToSQSServiceBus(SNSPublisher publisher, SequentialSQSReceiver receiver) : 
            this(publisher, receiver, true)
        {
            _publisher = publisher;
            _receiver = receiver;
        }

        public SNSToSQSServiceBus(SNSPublisher publisher, SequentialSQSReceiver receiver, bool createSubscription)
        {
            _publisher = publisher;
            _receiver = receiver;
            _createSubscription = createSubscription;
            _sQSReceiverThread = new Thread(_receiver.StartReceiving);
        }

        public bool IsReceiving
        {
            get { return _receiver.IsReceiving; }
        }

        public void StartReceiving()
        {
            if (!_configured)
            {
                Configure();
            }

            if (!_sQSReceiverThread.IsAlive)
            {
                _sQSReceiverThread.Start();
            }

        }

        public void StopReceiving()
        {
            _receiver.StopReceiving();
        }

        public void Publish(string message)
        {
            if (!_configured)
            {
                Configure();
            }

            _publisher.Publish(message);
        }

        public void Configure()
        {
            if (_configured)
            {
                return;
            }

            _publisher.Configure();
            _receiver.Configure();

            if (_createSubscription)
            {
                _publisher.Subscribe(_receiver.GetAmazonSQSClient(), _receiver.QueueUrl);
            }

            _configured = true;
        }
    }
}