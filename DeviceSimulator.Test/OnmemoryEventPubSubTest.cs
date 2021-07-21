namespace DeviceSimulator.Test
{
	using System;
	using System.Threading.Tasks;

	using Xunit;
	using PubSub;
	public class OnmemoryEventPubSubTest
	{
		private ITopicEventSubscriber subscriber { get; set; }
		private ITopicEventPublisher publisher { get; set; }
		public OnmemoryEventPubSubTest()
		{
			var hub = new Hub();
			this.subscriber = new OnmemoryEventSubscriber(hub);
			this.publisher = new OnmemoryEventPublisher(hub);
		}

		[Theory(Timeout = 100)]
		[InlineData("", "hello")]
		[InlineData("a", "hello")]
		[InlineData("a/b", "hello")]
		[InlineData("a/b/c/d", "hello")]

		public async Task TestSubscribeAllCausesMessageToBeReceived(string publishTopic, string message)
		{
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>("").GetAsyncEnumerator();
			await this.publisher.PublishAsync<string>(publishTopic, message);
			await messageAsyncEnumerator.MoveNextAsync();
			var value = messageAsyncEnumerator.Current;
			Assert.Equal(message, value.Message);
		}

		[Theory]
		[InlineData("a", "hello")]
		[InlineData("a/b", "hello")]
		[InlineData("a/b/c/d", "hello")]

		public async Task TestSubscribeNonEmptyTopicAndPublishEmptyTopicCausesMessageNotToBeReceived(string subscribeTopic, string message)
		{
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>(subscribeTopic).GetAsyncEnumerator();
			await this.publisher.PublishAsync<string>("", message);
			var messageTask = messageAsyncEnumerator.MoveNextAsync().AsTask();
			var delayTask = Task.Delay(100);
			var task = await Task.WhenAny(messageTask, delayTask);
			Assert.Equal(task, delayTask);
		}

	}
}
