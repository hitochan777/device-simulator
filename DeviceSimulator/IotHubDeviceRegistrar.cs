namespace DeviceSimulator
{
	using System.Collections.Generic;
	using Microsoft.Azure.Devices;

	public class IotHubDeviceRegistrar : IDeviceRegistrar
	{
		private RegistryManager registryManager { get; set; }
		public IotHubDeviceRegistrar(string connectionString)
		{
			this.registryManager = RegistryManager.CreateFromConnectionString(connectionString);
		}
		public async IAsyncEnumerable<string> FetchDevices()
		{
			var query = this.registryManager.CreateQuery("select * from devices", pageSize: 100);
			while (query.HasMoreResults)
			{
				var page = await query.GetNextAsTwinAsync();
				foreach (var twin in page)
				{
					yield return twin.DeviceId;
				}
			}
		}
	}
}