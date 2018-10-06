using Confluent.Kafka;
using Moq;

namespace Diadophis.Kafka.Test
{
    internal class FakeConsumerStrategy : IConsumerStrategy<string, string>
    {
        public IConsumer<string, string> Consumer { get; }

        public FakeConsumerStrategy()
        {
            Consumer = Mock.Of<IConsumer<string, string>>();
        }

        public void Close()
        {
            // Closing fake strategy...
        }

        public void Dispose()
        {
            // Disposing fake strategy...
        }
    }
}