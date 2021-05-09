namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	public interface IDeviceManager : IAsyncDisposable
	{
		public Task StartDeviceAsync(string deviceId);

		public Task StopDeviceAsync(string deviceId);
		public Task<IList<string>> ListDevices(bool onlyRunning);
		public Task CreateDeviceAsync(string deviceId);
		public Task RequestSendMessageAsync(string deviceId, string message);
	}
}
