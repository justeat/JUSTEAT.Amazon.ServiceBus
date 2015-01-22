namespace JUSTEAT.Amazon.ServiceBus
{
    public interface IServiceBus
    {
        bool IsReceiving { get; }
        void StartReceiving();
        void StopReceiving();
        void Publish(string message);
        void Configure();
    }
}
