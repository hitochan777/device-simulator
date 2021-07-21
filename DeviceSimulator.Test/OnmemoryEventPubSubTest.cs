namespace DeviceSimulator.Test
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using Xunit;
	using PubSub;
	public class OnmemoryEventPubSubTest
	{
		private Hub hub { get; set; }
		private ITopicEventSubscriber subscriber { get; set; }
		private ITopicEventPublisher publisher { get; set; }
		public OnmemoryEventPubSubTest()
		{
			this.hub = new Hub();
			this.subscriber = new OnmemoryEventSubscriber(this.hub);
			this.publisher = new OnmemoryEventPublisher(this.hub);
		}

		[Theory(Timeout = 100)]
		[InlineData("", "hello")]
		[InlineData("a", "hello")]
		[InlineData("a/b", "hello")]
		[InlineData("a/b/c/d", "hello")]

		public async Task TestSubscribeAllCausesMessageToBeReceived(string publishTopic, string message)
		{
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>("").GetAsyncEnumerator();
			var nextTask = messageAsyncEnumerator.MoveNextAsync();
			await this.publisher.PublishAsync<string>(publishTopic, message);
			await nextTask;
			var value = messageAsyncEnumerator.Current;
			Assert.Equal(message, value.Message);
		}

		[Theory]
		[InlineData("a", "hello")]
		[InlineData("a/b", "hello")]
		[InlineData("a/b/c/d", "hello")]

		public async Task TestSubscribeNonEmptyTopicAndPublishEmptyTopicCausesMessageNotToBeReceived(string subscribeTopic, string message)
		{
			CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken token = source.Token;
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>(subscribeTopic, token).GetAsyncEnumerator();
			await this.publisher.PublishAsync<string>("", message);
			var messageTask = messageAsyncEnumerator.MoveNextAsync().AsTask();
			var delayTask = Task.Delay(100);
			var task = await Task.WhenAny(messageTask, delayTask);
			Assert.Equal(task, delayTask);
		}

		[Theory(Timeout = 100)]
		[InlineData("a", "a", "good morning")]
		[InlineData("a", "a/b", "good morning")]
		[InlineData("a", "a/b/c", "good morning")]
		[InlineData("a/b", "a/b", "good morning")]
		[InlineData("a/b", "a/b/c", "good morning")]
		public async Task TestSubscribePartOfPublishTopicCausesMessageToBeReceived(string subscribeTopic, string publishTopic, string message)
		{
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>(subscribeTopic).GetAsyncEnumerator();
			var nextTask = messageAsyncEnumerator.MoveNextAsync();
			await this.publisher.PublishAsync<string>(publishTopic, message);
			await nextTask;
			var value = messageAsyncEnumerator.Current;
			Assert.Equal(message, value.Message);
		}

		[Theory]
		[InlineData("ab", "a", "good morning")]
		[InlineData("a/c", "a/b", "good morning")]
		[InlineData("a/b/d", "a/b/c", "good morning")]
		[InlineData("c", "a/b", "good morning")]
		public async Task TestSubscribeTopicNotMatcihngPublishTopicCausesMessageNotToBeReceived(string subscribeTopic, string publishTopic, string message)
		{
			CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken token = source.Token;
			var messageAsyncEnumerator = this.subscriber.SubscribeAsync<string>(subscribeTopic, token).GetAsyncEnumerator();
			await this.publisher.PublishAsync<string>(publishTopic, message);
			var messageTask = messageAsyncEnumerator.MoveNextAsync().AsTask();
			var delayTask = Task.Delay(100);
			var task = await Task.WhenAny(messageTask, delayTask);
			Assert.Equal(task, delayTask);
		}
	}
}
