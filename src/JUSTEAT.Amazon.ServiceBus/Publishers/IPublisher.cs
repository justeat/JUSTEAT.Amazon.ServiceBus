namespace JUSTEAT.Amazon.ServiceBus.Publishers
{
    public interface IPublisher
    {
        void Configure();
        void Publish(string message);
    }
}