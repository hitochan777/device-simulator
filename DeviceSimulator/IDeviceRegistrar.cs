namespace DeviceSimulator
{
	using System.Threading.Tasks;
	using System.Collections.Generic;
	public interface IDeviceRegistrar
	{
		IAsyncEnumerable<string> FetchDevicesAsync();
		Task<string> FetchDeviceAsync(string deviceId);
	}
}
