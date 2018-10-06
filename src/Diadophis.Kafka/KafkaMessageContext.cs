// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Confluent.Kafka;

namespace Diadophis.Kafka
{
    internal class KafkaMessageContext<TKey, TValue> : MessageContext
    {
        public KafkaMessageContext(IServiceProvider serviceProvider, ConsumeResult<TKey, TValue> consumeResult)
            : base(serviceProvider)
        {
            this.SetKafkaMessage(consumeResult);
        }
    }
}