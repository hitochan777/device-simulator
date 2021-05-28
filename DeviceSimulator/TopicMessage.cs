namespace DeviceSimulator
{
	public record TopicMessage<T>
	{

		public string Topic { get; set; }
	public T Message { get; set; }
}
}
