// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    internal class BasicConsumerStrategy<TKey, TValue> : IConsumerStrategy<TKey, TValue>
    {
        private bool _isDisposed = false;
        private readonly Consumer<TKey, TValue> _consumer;

        public IConsumer<TKey, TValue> Consumer => _consumer;

        public BasicConsumerStrategy(ConsumerConfig config)
        {
            _consumer = new Consumer<TKey, TValue>(config);
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
                    Consumer.Dispose();
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