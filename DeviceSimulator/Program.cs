namespace DeviceSimulator
{
	using System;
	using System.Threading.Tasks;
	class Program
	{
		private static readonly string CONNECTION_STRING = "HostName=attendance-taker-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=P0gtFyJAAeU0j+/j69hMoHBrlgksYlYq7I9BjJE8cpI=";
		static async Task Main(string[] args)
		{
			var deviceFactory = new IotHubDeviceFactory(CONNECTION_STRING);
			var deviceRegistrar = new IotHubDeviceRegistrar(CONNECTION_STRING);
			await using var deviceManager = new OnmemoryDeviceManager(deviceFactory, deviceRegistrar);
			while (true)
			{
				Console.Write("> ");
				var shouldQuit = false;
				var line = Console.ReadLine();
				var tokens = line.Split(' ');
				if (tokens.Length == 0)
				{
					Console.WriteLine("Please type command...");
					continue;
				}
				var cmd = tokens[0];
				switch (cmd)
				{
					case "start":
						{
							var deviceId = tokens[1];
							try
							{
								await deviceManager.StartDeviceAsync(deviceId);
							}
							catch (DeviceNotFoundException ex)
							{
								Console.WriteLine("Failed to start device: " + ex.Message);
							}
							catch (InvalidOperationException ex)
							{
								Console.WriteLine(ex.Message);
							}
							break;
						}
					case "stop":
						{
							var deviceId = tokens[1];
							try
							{
								await deviceManager.StopDeviceAsync(deviceId);
							}
							catch (InvalidOperationException ex)
							{
								Console.WriteLine(ex.Message);
							}
							break;
						}
					case "send":
						{
							var deviceId = tokens[1];
							var message = tokens[2];
							try
							{
								await deviceManager.RequestSendMessageAsync(deviceId, message);
							}
							catch (DeviceNotFoundException ex)
							{
								Console.WriteLine(ex.Message);
							}
							break;
						}
					case "list":
						{
							var ids = await deviceManager.ListDevices(onlyRunning: true);
							foreach (var id in ids)
							{
								Console.WriteLine(id);
							}
							break;
						}
					case "quit":
						shouldQuit = true;
						break;
					default:
						Console.WriteLine($"Invalid command: {cmd}.");
						break;
				}
				if (shouldQuit)
				{
					break;
				}
			}

		}
	}
}
