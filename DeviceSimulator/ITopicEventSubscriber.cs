namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	public interface ITopicEventSubscriber : IDisposable
	{
		IAsyncEnumerable<T> SubscribeAsync<T>(string topic, CancellationToken cancelToken);
	}
}
