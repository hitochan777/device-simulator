namespace DeviceSimulator
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Azure.Devices;
    using Azure.Core;

    public class IotHubDeviceRegistrar : IDeviceRegistrar
    {
        private RegistryManager registryManager { get; set; }
        public IotHubDeviceRegistrar(string connectionString)
        {
            this.registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }
        public IotHubDeviceRegistrar(string hostName, TokenCredential credential)
        {
            this.registryManager = RegistryManager.Create(hostName, credential);
        }

        public async IAsyncEnumerable<string> FetchDevicesAsync()
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

        public async Task<string> FetchDeviceAsync(string deviceId)
        {
            var device = await this.registryManager.GetDeviceAsync(deviceId);
            if (device == null)
            {
                return null;
            }
            return device.Id;
        }

    }
}
