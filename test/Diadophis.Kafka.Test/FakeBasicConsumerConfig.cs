namespace Diadophis.Kafka.Test
{
    internal class FakeBasicConsumerConfig : BasicConsumerConfig<string, string>
    {
        public FakeBasicConsumerConfig()
        {
            BrokerUrls = "broker1";
            ConsumerGroupId = "consumer";
            Topics = new[] { "topic" };
        }

        public override void ConfigurePipeline(IPipelineBuilder pipe)
        {
            // configure fake pipeline...
        }
    }
}
