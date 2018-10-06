// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace Diadophis.Kafka.Avro
{
    internal class AvroConsumerStrategy<TKey, TValue> : IConsumerStrategy<TKey, TValue>
    {
        private bool _isDisposed = false;
        private readonly AvroSerdeProvider _serdeProvider;
        private readonly Consumer<TKey, TValue> _consumer;

        public IConsumer<TKey, TValue> Consumer => _consumer;

        public AvroConsumerStrategy(ConsumerConfig config, AvroSerdeProviderConfig avroConfig)
        {
            _serdeProvider = new AvroSerdeProvider(avroConfig);
            _consumer = new Consumer<TKey, TValue>(config, 
                _serdeProvider.GetDeserializerGenerator<TKey>(), 
                _serdeProvider.GetDeserializerGenerator<TValue>());
        }

        public void Close()
        {
            _consumer.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _serdeProvider.Dispose();
                    _consumer.Dispose();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
