// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public interface IKafkaPipelineProvider
    {
        void Initialise(IKafkaConfig config);

        Task InvokePipeline<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult);
    }
}