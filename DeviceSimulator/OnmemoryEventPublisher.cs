namespace DeviceSimulator
{
	using System;
	using System.Threading.Tasks;

	using PubSub;
	public class OnmemoryEventPublisher : ITopicEventPublisher
	{
		private Hub hub { get; set; }

		public OnmemoryEventPublisher(Hub hub)
		{
			this.hub = hub;
		}
		public async Task PublishAsync<T>(string topic, T message)
		{
			await this.hub.PublishAsync(new TopicMessage<T> { Topic = topic, Message = message });
		}
	}
}
