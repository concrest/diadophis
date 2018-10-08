// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    internal class BasicProducerStrategy<TKey, TValue> : IProducerStrategy<TKey, TValue>
    {
        private bool _isDisposed = false;
        public IProducer<TKey, TValue> Producer { get; }

        public BasicProducerStrategy(ProducerConfig config)
        {
            Producer = new Producer<TKey, TValue>(config);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Producer.Dispose();
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