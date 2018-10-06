using System;
using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IConsumerStrategy<TKey, TValue> : IDisposable
    {
        IConsumer<TKey, TValue> Consumer { get; }

        // IConsumer<> doesn't expose .Close, so expose it here
        void Close();
    }
}
