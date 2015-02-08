using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace JUSTEAT.Amazon.ServiceBus.Receivers
{
    public class ConcurrentSqsReceiver : SequentialSQSReceiver
    {
        private readonly List<Task> _activeTasks;
        private long _activeTaskCount;
        private readonly int _maximumNumberOfTasks;

        public ConcurrentSqsReceiver(IAmazonSQS amazonSQSClient, string queueUrl, Func<Message, bool> onReceived,
            Action<Exception, Message> onError, int maxNumberOfMessages = 1, int concurrency = 1)
            : this(amazonSQSClient, new QueueConfiguration(queueUrl), onReceived, onError, maxNumberOfMessages, concurrency)
        {
        }

        public ConcurrentSqsReceiver(IAmazonSQS amazonSQSClient, QueueByName queueByName, Func<Message, bool> onReceived,
            Action<Exception, Message> onError, int maxNumberOfMessages = 1, int concurrency = 1)
            : this(amazonSQSClient, new QueueConfiguration(amazonSQSClient, queueByName), onReceived, onError, maxNumberOfMessages, concurrency)
        {
        }

        public ConcurrentSqsReceiver(IAmazonSQS amazonSQSClient, QueueConfiguration queueConfiguration, Func<Message, bool> onReceived,
            Action<Exception, Message> onError, int maxNumberOfMessages = 1, int concurrency = 1)
            : base(amazonSQSClient, queueConfiguration, onReceived, onError, maxNumberOfMessages)
        {
            if (concurrency < 1)
            {
                throw new ArgumentException("Concurrency must be greater than 0", "concurrency");
            }

            _activeTasks = new List<Task>();
            _maximumNumberOfTasks = MaxNumberOfMessages*concurrency;
        }

        protected override void OnBeforeReceiveMessages()
        {
            while (Interlocked.Read(ref _activeTaskCount) >= _maximumNumberOfTasks)
            {
                Task[] activeTasksToWaitOn;
                lock (_activeTasks)
                {
                    activeTasksToWaitOn = _activeTasks.Where(x => x != null).ToArray();
                }

                if (activeTasksToWaitOn.Length == 0)
                {
                    continue;
                }

                Task.WaitAny(activeTasksToWaitOn);
            }
        }

        protected override void OnReceivedMessage(Message message)
        {
            var action = new Action(() => ProcessMessage(message));
            var task = new Task(action);
            task.ContinueWith(MarkTaskAsComplete, TaskContinuationOptions.ExecuteSynchronously);
            task.Start();
        }

        public override void StopReceiving()
        {
            Task[] activeTasksToWaitOn;
            lock (_activeTasks)
            {
                activeTasksToWaitOn = _activeTasks.Where(x => x != null).ToArray();
            }

            if (activeTasksToWaitOn.Length > 0)
            {
                Task.WaitAll(activeTasksToWaitOn);
            }

            base.StopReceiving();
        }

        private void MarkTaskAsComplete(Task t)
        {
            lock (_activeTasks)
            {
                _activeTasks.Remove(t);
            }

            Interlocked.Decrement(ref _activeTaskCount);
        }
    }
}