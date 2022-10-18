namespace DeviceSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    public interface ITopicEventSubscriber : IDisposable
    {
        IAsyncEnumerable<TopicMessage<T>> SubscribeAsync<T>(string topic, CancellationToken cancelToken = default);
    }
}
