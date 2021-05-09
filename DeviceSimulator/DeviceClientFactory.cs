namespace DeviceSimulator
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Azure.Devices;
	using Microsoft.Azure.Devices.Client;

	public class DeviceClientFactory
	{
		private RegistryManager registryManager { get; set; }
		public DeviceClientFactory(string connectionString)
		{
			this.registryManager = RegistryManager.CreateFromConnectionString(connectionString);
		}

		public async Task<DeviceClient> CreateDeviceClient(string deviceId)
		{
			var device = await this.registryManager.GetDeviceAsync(deviceId);
			if (device.ConnectionState == DeviceConnectionState.Connected)
			{
				throw new InvalidOperationException($"{deviceId} is in connected state. You need to disconnect the device to create an instance for this device.");
			}
			var connectionString = ""; // TODO: create from device info
			return DeviceClient.CreateFromConnectionString(connectionString);
		}
	}
}