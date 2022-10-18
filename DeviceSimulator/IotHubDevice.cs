namespace DeviceSimulator
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text;
    using Microsoft.Azure.Devices.Client;
    public class IotHubDevice : IDevice
    {
        public event EventHandler<byte[]> MessageReceived;
        private string deviceId { get; set; }
        private DeviceClient deviceClient { get; set; }
        private Task receiverTask { get; set; }
        private CancellationTokenSource cancellationTokenSource { get; set; }
        private ITopicEventPublisher eventPublisher { get; set; }


        public IotHubDevice(string deviceId, DeviceClient deviceClient, ITopicEventPublisher eventPublisher)
        {
            this.deviceId = deviceId;
            this.deviceClient = deviceClient;
            this.eventPublisher = eventPublisher;
        }

        public Task StartAsync(CancellationToken token)
        {
            if (receiverTask != null)
            {
                throw new InvalidOperationException("device is running");
            }
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            this.receiverTask = StartReceiverAsync(this.cancellationTokenSource.Token);
            return this.receiverTask;
        }

        public async Task SendMessageAsync(byte[] message)
        {
            await this.deviceClient.SendEventAsync(new Message(message));
            Console.WriteLine($"Sent message {message}");
        }

        public async Task StopAsync()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }
            if (this.receiverTask != null)
            {
                await this.receiverTask;
            }
            this.receiverTask = null;
            this.deviceClient.Dispose();
        }

        private async Task StartReceiverAsync(CancellationToken token)
        {
            Console.WriteLine($"[{this.deviceId}] waiting for message...");
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // https://github.com/Azure/azure-iot-sdk-csharp/issues/1921
                    // var message = await this.deviceClient.ReceiveAsync(token);
                    var message = await this.deviceClient.ReceiveAsync(TimeSpan.FromMilliseconds(1000));
                    if (message == null)
                    {
                        continue;
                    }
                    // await this.deviceClient.CompleteAsync(message, token);
                    await this.deviceClient.CompleteAsync(message);
                    var bytes = message.GetBytes();
                    Console.WriteLine($"\n[{this.deviceId}] Received message: {BitConverter.ToString(bytes)}");
                    await this.eventPublisher.PublishAsync($"{deviceId}/receive-c2d", bytes);

                    EventHandler<byte[]> handler = MessageReceived;
                    if (handler is not null)
                    {
                        handler(this, bytes);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            await this.eventPublisher.PublishAsync("stop-receiver", deviceId);
            Console.WriteLine($"[{this.deviceId}] stopping receiver");
        }
        public void RegisterJob(Func<IDevice, Func<CancellationToken, Task>> jobCreator)
        {
            var job = jobCreator(this);
            _ = job(this.cancellationTokenSource.Token);
        }
    }
}
