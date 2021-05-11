namespace DeviceSimulator
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Threading.Channels;

	using PubSub;
	public class OnmemoryEventSubscriber : ITopicEventSubscriber
	{
		private Hub hub { get; set; }

		public OnmemoryEventSubscriber(Hub hub)
		{
			this.hub = hub;
		}
		public async IAsyncEnumerable<T> SubscribeAsync<T>(string topic, CancellationToken cancelToken)
		{
			var channel = Channel.CreateUnbounded<T>();
			Action<TopicMessage<T>> handler = async (message) =>
			{
				var messageTopic = message.Topic.Split('/');
				var targetTopic = topic.Split('/');
				if (targetTopic.Length > messageTopic.Length)
				{
					return;
				}
				var n = targetTopic.Length;
				for (int i = 0; i < n; i++)
				{
					if (targetTopic[i] != messageTopic[i])
					{
						return;
					}
				}
				await channel.Writer.WriteAsync(message.Message);
			};
			this.hub.Subscribe<TopicMessage<T>>(handler);
			await foreach (var data in channel.Reader.ReadAllAsync(cancelToken))
			{
				yield return data;
			}
			this.hub.Unsubscribe<TopicMessage<T>>(handler);
		}

		public void Dispose()
		{
		}
	}
}
