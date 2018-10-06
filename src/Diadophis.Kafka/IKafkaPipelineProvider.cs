// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IKafkaPipelineProvider<TKey, TValue>
    {
        void Initialise(IKafkaConfig<TKey, TValue> config);

        Task InvokePipeline(ConsumeResult<TKey, TValue> consumeResult);
    }
}