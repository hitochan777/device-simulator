namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	public interface IDeviceManager : IAsyncDisposable
	{
		public Task StartDeviceAsync(string deviceId);

		public Task StopDeviceAsync(string deviceId);
		public Task<IList<DeviceStatus>> GetDeviceStatusesAsync();
		public Task<DeviceStatus> GetDeviceStatusAsync(string deviceId);
		public Task CreateDeviceAsync(string deviceId);
		public Task RequestSendMessageAsync(string deviceId, byte[] message);

		public IAsyncEnumerable<TopicMessage<T>> Subscribe<T>(string topic);
	}
}
