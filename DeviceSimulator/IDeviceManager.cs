namespace DeviceSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    public interface IDeviceManager : IAsyncDisposable
    {
        public Task StartDeviceAsync(string deviceId);

        public Task StopDeviceAsync(string deviceId);
        public Task<IList<DeviceStatus>> GetDeviceStatusesAsync();
        public Task<DeviceStatus> GetDeviceStatusAsync(string deviceId);
        public Task CreateDeviceAsync(string deviceId);
        public Task RequestSendMessageAsync(string deviceId, byte[] message);
        public void AddReceiveMessageEventHandler(string deviceId, EventHandler<byte[]> handler);
        /// <summary>
        /// Register job to run on a device
        /// </summary>
        /// <param name="deviceId">DeviceId to register job for</param>
        /// <param name="jobCreator">Factory function that returns async job function</param>
        public void RegisterJob(string deviceId, Func<IDevice, Func<CancellationToken, Task>> jobCreator);

        public IAsyncEnumerable<TopicMessage<T>> Subscribe<T>(string topic);
    }
}
