// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IProducerStrategy<TKey, TValue> : IDisposable
    {
        IProducer<TKey, TValue> Producer { get; }
    }
}