using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DeviceSimulator
{
	public class DeviceManager : IDeviceManager
	{
		private Dictionary<string, IDevice> devices { get; set; }
        private CancellationTokenSource cancellationTokenSource {get; set;}
        private DeviceClientFactory deviceClientFactory {get; set;}

		public DeviceManager(string connectionString)
		{
			this.devices = new Dictionary<string, IDevice>();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.deviceClientFactory = new DeviceClientFactory(connectionString);
		}
        
		public async Task StartDeviceAsync(string deviceId)
		{
            if (devices.ContainsKey(deviceId)) {
                throw new InvalidOperationException($"{deviceId} is already runnning");
            }
            var deviceClient = await this.deviceClientFactory.CreateDeviceClient(deviceId);
			var device = new IotHubDevice(deviceId, deviceClient);
            await device.StartAsync(this.cancellationTokenSource.Token);
		}

		public async Task CreateDeviceAsync(string deviceId)
		{
            // TODO: Not Implemented
            await Task.CompletedTask;
		}

		public async Task RequestSendMessageAsync(string deviceId, string message)
		{
			IDevice device;
			var ok = this.devices.TryGetValue(deviceId, out device);
			if (!ok)
			{
				throw new InvalidOperationException($"Device (ID: {deviceId}) has not been started yet.");
			}
			await device.SendMessageAsync(message);
		}
	}
}