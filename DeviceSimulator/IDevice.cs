using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public interface IDevice
    {
        event EventHandler<byte[]> MessageReceived;
        Task StartAsync(CancellationToken token);

        Task StopAsync();
        Task SendMessageAsync(byte[] message);
        void RegisterJob(Func<IDevice, Func<CancellationToken, Task>> jobCreator);
    }
}
