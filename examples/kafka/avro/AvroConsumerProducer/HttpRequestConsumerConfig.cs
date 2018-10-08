// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Diadophis;
using Diadophis.Kafka.Avro;
using SchemaExamples.Model;

namespace AvroConsumerProducer
{
    internal class HttpRequestConsumerConfig : AvroConsumerConfig<string, HttpRequest>
    {
        public override void ConfigurePipeline(IPipelineBuilder pipe)
        {
            pipe
                .UseMiddleware<MapRequestToPageViewMiddleware>()
                .UseMiddleware<CleanUserAgentMiddleware>()
                .UseMiddleware<PublishPageViewMiddleware>();
        }
    }
}