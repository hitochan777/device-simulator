namespace Schema.Query
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DeviceSimulator;
    using Schema.Object;
    public class Query 
    {
        private IDeviceManager deviceManager {get; set;}
        public Query(IDeviceManager deviceManager) {
            this.deviceManager = deviceManager;
        }
        public async Task<IList<Device>> ListDevices() {
            var deviceIds = await this.deviceManager.ListDevices(true);
            return deviceIds.Select(id => new Device {Id = id}).ToList();
        }
    }
}
