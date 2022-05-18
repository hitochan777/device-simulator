using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using DeviceSimulator;

using PubSub;

var ENDPOINT = Environment.GetEnvironmentVariable("IOTHUB_ENDPOINT");
var credential = new DefaultAzureCredential();

var deviceFactory = new IotHubDeviceFactory(ENDPOINT, credential);
var deviceRegistrar = new IotHubDeviceRegistrar(ENDPOINT, credential);

var hub = new Hub();
var eventPublisher = new OnmemoryEventPublisher(hub);
var eventSubscriber = new OnmemoryEventSubscriber(hub);

await using var deviceManager = new OnmemoryDeviceManager(deviceFactory, deviceRegistrar, eventPublisher, eventSubscriber);
_ = Task.Run(async () =>
    {
      await foreach (var message in deviceManager.Subscribe<byte[]>(""))
      {
        var text = Encoding.UTF8.GetString(message.Message);
        Console.WriteLine($"[{message.Topic}] {text}");
      }
    });
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
          await deviceManager.RequestSendMessageAsync(deviceId, Encoding.ASCII.GetBytes(message));
        }
        catch (DeviceNotFoundException ex)
        {
          Console.WriteLine(ex.Message);
        }
        break;
      }
    case "list":
      {
        var statuses = await deviceManager.GetDeviceStatusesAsync();
        foreach (var status in statuses)
        {
          Console.WriteLine($"{status.Id}: {status.IsRunning}");
        }
        break;
      }
    case "show":
      {
        var deviceId = tokens[1];
        var status = await deviceManager.GetDeviceStatusAsync(deviceId);
        Console.WriteLine($"{status.Id}: {status.IsRunning}");
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