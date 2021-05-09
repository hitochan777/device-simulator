namespace Schema.Mutation
{
    using System.Threading.Tasks;

    using DeviceSimulator;
    using Schema.Object;
    public class Mutation
    {
        private IDeviceManager deviceManager {get; set;}
        public Mutation(IDeviceManager deviceManager) {
            this.deviceManager = deviceManager;
        }
        public async Task<Device> StartDevice(string deviceId) {
            await this.deviceManager.StartDeviceAsync(deviceId);
            return new Device { Id = deviceId };
        }

        public async Task<Device> StopDevice(string deviceId) {
            await this.deviceManager.StopDeviceAsync(deviceId);
            return new Device { Id = deviceId };
        }         
        public async Task<Device> RequestSendMessage(string deviceId, string message) {
            await this.deviceManager.RequestSendMessageAsync(deviceId, message);
            return new Device { Id = deviceId };
        }
    }
}
