namespace DeviceSimulator
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Collections.Generic;
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
		public async IAsyncEnumerable<TopicMessage<T>> SubscribeAsync<T>(string topic, [EnumeratorCancellation] CancellationToken cancelToken)
		{
			var channel = Channel.CreateUnbounded<TopicMessage<T>>();
			Action<TopicMessage<T>> handler = async (message) =>
			{
				var messageTopic = string.IsNullOrEmpty(message.Topic) ? new string[] { } : message.Topic.Split('/');
				var targetTopic = string.IsNullOrEmpty(topic) ? new string[] { } : topic.Split('/');
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
				await channel.Writer.WriteAsync(message);
			};
			Console.WriteLine("subscribing");
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
