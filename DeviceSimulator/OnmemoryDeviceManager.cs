namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Concurrent;

	public class OnmemoryDeviceManager : IDeviceManager
	{
		private ConcurrentDictionary<string, IDevice> devices { get; set; }
		private CancellationTokenSource cancellationTokenSource { get; set; }
		private IDeviceFactory deviceFactory { get; set; }
		private IDeviceRegistrar deviceRegistrar { get; set; }

		public OnmemoryDeviceManager(IDeviceFactory deviceFactory, IDeviceRegistrar deviceRegistrar)
		{
			this.devices = new ConcurrentDictionary<string, IDevice>();
			this.cancellationTokenSource = new CancellationTokenSource();
			this.deviceFactory = deviceFactory;
			this.deviceRegistrar = deviceRegistrar;
		}

		public async Task StartDeviceAsync(string deviceId)
		{
			if (devices.ContainsKey(deviceId))
			{
				throw new InvalidOperationException($"{deviceId} is already runnning");
			}
			var device = await this.deviceFactory.CreateDevice(deviceId);
			if (device == null)
			{
				throw new DeviceNotFoundException(deviceId);
			}
			await device.StartAsync(this.cancellationTokenSource.Token);
			devices[deviceId] = device;
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

		public async Task<IList<string>> ListDevices(bool onlyRunning = true)
		{
			if (onlyRunning)
			{
				return this.devices.Keys.ToList();
			}
			var ids = new List<string>();
			await foreach (var deviceId in this.deviceRegistrar.FetchDevices())
			{
				ids.Add(deviceId);
			}
			return ids;
		}

		public void Dispose()
		{
			this.cancellationTokenSource.Cancel();
		}
	}
}
