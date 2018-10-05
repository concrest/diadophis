// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis;
using Diadophis.Kafka;
using SchemaExamples.Model;

namespace SingleSchemaConsumer
{
    internal class AvroKafkaConfig : BasicConsumerConfig<string, HttpRequest>
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