namespace DeviceSimulator
{
	using System.Collections.Generic;
	public interface IDeviceRegistrar
	{
		IAsyncEnumerable<string> FetchDevices();
	}
}
