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

		public IotHubDevice(string deviceId, DeviceClient deviceClient)
		{
			this.deviceClient = deviceClient;
		}

		public Task StartAsync(CancellationToken token)
		{
			_ = StartReceiverAsync(token);
			return Task.CompletedTask;
		}

		public async Task SendMessageAsync(string message)
		{
			await this.deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(message)));
			Console.WriteLine($"Sent message {message}");
		}

		private async Task StartReceiverAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				Console.WriteLine("waiting for message...");
				var message = await this.deviceClient.ReceiveAsync(token);
				await this.deviceClient.CompleteAsync(message.LockToken);
				Console.WriteLine("Received message ");
			}
			Console.WriteLine("stopping receiver");
		}
	}
}
