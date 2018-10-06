// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Diadophis;
using Diadophis.Kafka;
using Diadophis.Kafka.Avro;
using SchemaExamples.Model;

namespace SingleSchemaConsumer
{
    internal class AvroKafkaConfig : AvroConsumerConfig<string, HttpRequest>
    {
        public override void ConfigurePipeline(IPipelineBuilder pipe)
        {
           pipe.Run(context =>
           {
               var message = context.GetKafkaMessage<string, HttpRequest>();

               return Task.CompletedTask;
           });
        }
    }
}