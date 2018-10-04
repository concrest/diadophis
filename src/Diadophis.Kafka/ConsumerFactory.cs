using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public class ConsumerFactory : IConsumerFactory
    {
        public IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(ConsumerConfig config)
        {
            return new Consumer<TKey, TValue>(config);
        }
    }
}
