using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
	public interface IDevice
	{
		Task StartAsync(CancellationToken token);
		Task SendMessageAsync(string message);
	}
}
