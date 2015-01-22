using System;
using Amazon.Runtime.Internal.Util;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace JUSTEAT.Amazon.ServiceBus.Receivers
{
    public class SequentialSQSReceiver : IReceiver
    {
        private readonly IAmazonSQS _amazonSQSClient;
        private readonly Func<Message, bool> _onRecieved;
        private readonly Action<Exception, Message> _onError;
        private const int DefaultNumberOfMessagesToReceiveAtATime = 1;
        private static readonly Logger Logger = Logger.GetLogger(typeof(SequentialSQSReceiver));
        
        protected readonly int MaxNumberOfMessages;
        private readonly QueueConfiguration _queueConfiguration;
        private bool _configured;

        public bool IsReceiving { get; private set; }
        public string QueueUrl { get; private set; }

        public SequentialSQSReceiver(IAmazonSQS amazonSQSClient, string queueUrl, Func<Message, bool> onRecieved,
            Action<Exception, Message> onError, int maxNumberOfMessages = DefaultNumberOfMessagesToReceiveAtATime) :
            this(amazonSQSClient, new QueueConfiguration(queueUrl), onRecieved, onError, maxNumberOfMessages)
        {
        }

        public SequentialSQSReceiver(IAmazonSQS amazonSQSClient, QueueByName queueByName, Func<Message, bool> onRecieved,
            Action<Exception, Message> onError, int maxNumberOfMessages = DefaultNumberOfMessagesToReceiveAtATime) :
            this(amazonSQSClient, new QueueConfiguration(amazonSQSClient, queueByName), onRecieved, onError, maxNumberOfMessages)
        {
        }

        public SequentialSQSReceiver(IAmazonSQS amazonSQSClient, QueueConfiguration queueConfiguration, Func<Message, bool> onRecieved, Action<Exception, Message> onError, int maxNumberOfMessages = DefaultNumberOfMessagesToReceiveAtATime)
        {
            if (maxNumberOfMessages > 10 | maxNumberOfMessages < 1)
            {
                throw new ArgumentException("MaxNumberOfMessages must be between 1 and 10", "maxNumberOfMessages");
            }
            
            _amazonSQSClient = amazonSQSClient;
            _queueConfiguration = queueConfiguration;
            MaxNumberOfMessages = maxNumberOfMessages;
            _onRecieved = onRecieved;
            _onError = onError;
        }

        public void StartReceiving()
        {
            if (!_configured)
            {
                Configure();
            }

            IsReceiving = true;
            
            while (IsReceiving)
            {
                OnBeforeReceiveMessages();

                var receiveMessageResponse = _amazonSQSClient.ReceiveMessage(new ReceiveMessageRequest
                {
                    QueueUrl = QueueUrl,
                    MaxNumberOfMessages = MaxNumberOfMessages,
                    WaitTimeSeconds = 20,
                    
                });

                receiveMessageResponse.Messages.ForEach(OnReceivedMessage);
            }
        }

        protected virtual void OnBeforeReceiveMessages()
        {
        }

        protected virtual void OnReceivedMessage(Message message)
        {
            ProcessMessage(message);
        }

        public virtual void StopReceiving()
        {
            IsReceiving = false;
        }

        protected void ProcessMessage(Message message)
        {
            var handledSuccessfully = false;
            try
            {
                handledSuccessfully = _onRecieved.Invoke(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception occured whilst processing message.");

                try
                {
                    if (_onError != null)
                    {
                        _onError(ex, message);
                    }
                }
                catch (Exception onErrorException)
                {
                    Logger.Error(onErrorException, "Exception during invocation of onError action.");
                }
            }
            finally
            {
                if (handledSuccessfully)
                {
                    _amazonSQSClient.DeleteMessage(new DeleteMessageRequest
                    {
                        QueueUrl = QueueUrl,
                        ReceiptHandle = message.ReceiptHandle,
                    });
                }
            }
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

        internal IAmazonSQS GetAmazonSQSClient()
        {
            return _amazonSQSClient;
        }
    }
}
