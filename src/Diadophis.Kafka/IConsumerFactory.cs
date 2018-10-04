using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IConsumerFactory
    {
        IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config);
    }
}
