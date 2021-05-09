namespace DeviceSimulator
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Azure.Devices;
	using Microsoft.Azure.Devices.Client;

	public class IotHubDeviceFactory : IDeviceFactory
	{
		private RegistryManager registryManager { get; set; }
		public IotHubDeviceFactory(string connectionString)
		{
			this.registryManager = RegistryManager.CreateFromConnectionString(connectionString);
		}

		public async Task<IDevice> CreateDevice(string deviceId)
		{
			var device = await this.registryManager.GetDeviceAsync(deviceId);
			if (device == null)
			{
				return null;
			}
			if (device.ConnectionState == DeviceConnectionState.Connected)
			{
				throw new InvalidOperationException($"{deviceId} is in connected state. You need to disconnect the device to create an instance for this device.");
			}
			var connectionString = ""; // TODO: create from device info
			var deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
			return new IotHubDevice(deviceId, deviceClient);
		}
	}
}
