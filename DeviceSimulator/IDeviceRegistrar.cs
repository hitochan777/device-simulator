namespace DeviceSimulator
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	public interface IDeviceRegistrar
	{
		IAsyncEnumerable<string> FetchDevices();
	}
}
