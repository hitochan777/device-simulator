using System.Threading.Tasks;

namespace DeviceSimulator
{
	public interface IDeviceManager
	{
		public Task StartDeviceAsync(string deviceId);
		public Task CreateDeviceAsync(string deviceId);
		public Task RequestSendMessageAsync(string deviceId, string message);
	}
}
