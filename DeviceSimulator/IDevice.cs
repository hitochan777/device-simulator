using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
	public interface IDevice
	{
		Task StartAsync(CancellationToken token);

		Task StopAsync();
		Task SendMessageAsync(byte[] message);
	}
}
