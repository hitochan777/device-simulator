namespace DeviceSimulator
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.Azure.Devices;
	using Microsoft.Azure.Devices.Client;

	public class IotHubDeviceFactory : IDeviceFactory
	{
		private RegistryManager registryManager { get; set; }
		private readonly static string CONNECTION_STRING_TEMPLATE = "HostName={0};DeviceId={1};SharedAccessKey={2}";
		private string hostName { get; set; }
		public IotHubDeviceFactory(string connectionString)
		{
			this.registryManager = RegistryManager.CreateFromConnectionString(connectionString);

			// extract hostname from connection string
			// Assumes the following format: HostName=hohgehoge.azure-devices.net;....;...
			// We want hogehoge.azure-devices.net part
			this.hostName = connectionString.Split(';').Where(term => term.StartsWith("HostName")).FirstOrDefault()?.Split('=').LastOrDefault();
			if (string.IsNullOrEmpty(this.hostName))
			{
				throw new ArgumentException("Host name could not be extracted from connection string.");
			}
		}

		private string BuildConnectionString(string deviceId, string key)
		{
			return string.Format(CONNECTION_STRING_TEMPLATE, this.hostName, deviceId, key);
		}

		public async Task<IDevice> CreateDevice(string deviceId, ITopicEventPublisher eventPublisher)
		{
			var device = await this.registryManager.GetDeviceAsync(deviceId);
			if (device == null)
			{
				return null;
			}
			var key = device.Authentication.SymmetricKey.PrimaryKey;
			var connectionString = this.BuildConnectionString(deviceId, key);
			var deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
			return new IotHubDevice(deviceId, deviceClient, eventPublisher);
		}
	}
}
