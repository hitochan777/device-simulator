namespace DeviceSimulator
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Text;
	using Microsoft.Azure.Devices.Client;
	public class IotHubDevice : IDevice
	{
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

		public async Task SendMessageAsync(string message)
		{
			await this.deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(message)));
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
					var text = Encoding.UTF8.GetString(bytes);
					Console.WriteLine($"\n[{this.deviceId}] Received message: {text}");
					await this.eventPublisher.PublishAsync($"{deviceId}/receive-c2d", bytes);
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

	}
}
