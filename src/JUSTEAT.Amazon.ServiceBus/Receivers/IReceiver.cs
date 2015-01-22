namespace JUSTEAT.Amazon.ServiceBus.Receivers
{
    public interface IReceiver
    {
        void Configure();
        void StartReceiving();
        void StopReceiving();
        bool IsReceiving { get; }
    }
}