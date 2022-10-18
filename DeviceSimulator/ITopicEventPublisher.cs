namespace DeviceSimulator
{
    using System.Threading.Tasks;
    public interface ITopicEventPublisher
    {
        Task PublishAsync<T>(string topic, T message);
    }
}
