namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Concurrent;

	public class TopicValue<T>
	{
		public string Topic { get; set; }
		public T Value { get; set; }
	}

	public class OnmemoryDeviceManager : IDeviceManager
	{
		private ConcurrentDictionary<string, IDevice> devices { get; set; }
		private CancellationTokenSource cancellationTokenSource { get; set; }
		private IDeviceFactory deviceFactory { get; set; }
		private IDeviceRegistrar deviceRegistrar { get; set; }
		private ITopicEventPublisher eventPublisher { get; set; }

		private ITopicEventSubscriber eventSubscriber { get; set; }

		public OnmemoryDeviceManager(IDeviceFactory deviceFactory, IDeviceRegistrar deviceRegistrar, ITopicEventPublisher eventPublisher, ITopicEventSubscriber eventSubscriber)
		{
			this.devices = new ConcurrentDictionary<string, IDevice>();
			this.cancellationTokenSource = new CancellationTokenSource();
			this.deviceFactory = deviceFactory;
			this.deviceRegistrar = deviceRegistrar;
			this.eventPublisher = eventPublisher;
			this.eventSubscriber = eventSubscriber;
			_ = this.WatchDevices();
		}

		public async Task WatchDevices()
		{
			await foreach (var ev in this.eventSubscriber.SubscribeAsync<string>("stop-receiver").WithCancellation(this.cancellationTokenSource.Token))
			{
				var deviceId = ev.Message;
				this.devices.Remove(deviceId, out _);
			}
		}

		public async Task StartDeviceAsync(string deviceId)
		{
			if (devices.ContainsKey(deviceId))
			{
				throw new InvalidOperationException($"{deviceId} is already runnning");
			}
			var device = await this.deviceFactory.CreateDevice(deviceId, eventPublisher);
			if (device == null)
			{
				throw new DeviceNotFoundException(deviceId);
			}
			_ = device.StartAsync(this.cancellationTokenSource.Token);
			devices[deviceId] = device;
		}

		public async Task StopDeviceAsync(string deviceId)
		{
			if (!devices.ContainsKey(deviceId))
			{
				throw new InvalidOperationException($"{deviceId} is not started yet");
			}
			await devices[deviceId].StopAsync();
		}

		public async Task CreateDeviceAsync(string deviceId)
		{
			// TODO: Not Implemented
			await Task.CompletedTask;
		}

		public async Task RequestSendMessageAsync(string deviceId, byte[] message)
		{
			IDevice device;
			var ok = this.devices.TryGetValue(deviceId, out device);
			if (!ok)
			{
				throw new InvalidOperationException($"Device (ID: {deviceId}) has not been started yet.");
			}
			await device.SendMessageAsync(message);
		}

		public async Task<IList<DeviceStatus>> GetDeviceStatusesAsync()
		{
			var ids = new List<DeviceStatus>();
			await foreach (var deviceId in this.deviceRegistrar.FetchDevicesAsync())
			{
				ids.Add(new DeviceStatus
				{
					Id = deviceId,
					IsRunning = this.devices.ContainsKey(deviceId)
				});
			}
			return ids;
		}

		public async Task<DeviceStatus> GetDeviceStatusAsync(string deviceId)
		{
			var device = await this.deviceRegistrar.FetchDeviceAsync(deviceId);
			if (device == null)
			{
				throw new ArgumentException($"{deviceId} is not found in device registrar");
			}
			return await Task.FromResult(new DeviceStatus
			{
				Id = deviceId,
				IsRunning = this.devices.ContainsKey(deviceId)
			});
		}

		public IAsyncEnumerable<TopicMessage<T>> Subscribe<T>(string topic)
		{
			return this.eventSubscriber.SubscribeAsync<T>(topic);
		}

		public async ValueTask DisposeAsync()
		{
			this.cancellationTokenSource.Cancel();
			await Task.WhenAll(this.devices.Values.Select(device => device.StopAsync()));
			this.devices = default;
			Console.WriteLine("disposed device manager...");
		}
	}
}
