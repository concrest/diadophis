// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Diadophis.Kafka;
using SchemaExamples.Model;

namespace AvroConsumerProducer
{
    internal class PageViewProducerConfig : BasicProducerConfig<string, PageView>
    {
        // TODO - This needs to be an AvroProductConfig subclass.  Need to create that base class 
        // and the strategy for it before we can.
    }
}