using System;
using System.Threading;
using JUSTEAT.Amazon.ServiceBus.Publishers;
using JUSTEAT.Amazon.ServiceBus.Receivers;

namespace JUSTEAT.Amazon.ServiceBus
{
    public class SQSToSQSServiceBus : IServiceBus
    {
        private readonly SQSPublisher _publisher;
        private readonly SequentialSQSReceiver _receiver;
        private readonly Thread _sQSReceiverThread;

        private bool _configured;

        public SQSToSQSServiceBus(SQSPublisher publisher, SequentialSQSReceiver receiver)
        {
            _publisher = publisher;
            _receiver = receiver;
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

            ThrowExceptionIfThePublisherAndRecieverQeuesAreDifferent();

            _configured = true;
        }

        private void ThrowExceptionIfThePublisherAndRecieverQeuesAreDifferent()
        {
            if (!_publisher.QueueUrl.Equals(_receiver.QueueUrl))
            {
                throw new InvalidOperationException(
                    "Amazon SQS does not currently support publishing from a queue into a different queue.  " +
                    "Please ensure both the SQSPublisher and SQSReciever are configured to use the same Queue URL or Queue Name.");

            }
        }
    }
}