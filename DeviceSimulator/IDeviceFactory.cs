namespace DeviceSimulator
{
	using System.Threading.Tasks;
	public interface IDeviceFactory
	{
		Task<IDevice> CreateDevice(string deviceId, ITopicEventPublisher eventPublisher);
	}
}
