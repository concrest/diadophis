// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using SchemaExamples.Model;

namespace AvroMessageProducer
{
    class Program
    {
        static long UnixNow() => (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

        static void Main(string[] args)
        {
            int messages = 1;

            if (args.Length > 0)
            {
                messages = int.Parse(args[0]);
            }

            var config = new ProducerConfig
            {
                BootstrapServers = "broker1:9092",
            };

            var avroConfig = new AvroSerdeProviderConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                SchemaRegistryUrl = "http://schemaregistry1:8081",
                // optional schema registry client properties:
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10,
                // optional avro serializer properties:
                AvroSerializerBufferBytes = 50,
                AvroSerializerAutoRegisterSchemas = true
            };

            using (var serdeProvider = new AvroSerdeProvider(avroConfig))
            using (var producer = new Producer<string, HttpRequest>(
                config,
                serdeProvider.GetSerializerGenerator<string>(),
                serdeProvider.GetSerializerGenerator<HttpRequest>()))
            {
                var httpRequest = new HttpRequest
                {
                    request_verb = HttpVerbs.Get,
                    path = "/index.html",
                    timestamp = UnixNow(),
                    duration = 12.3,
                    user_agent = "Foo"
                };


                for (int i = 0; i < messages; i++)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Sending message {i + 1} of {messages}");
                    // Alternatively - await ProduceAsync
                    producer.BeginProduce(
                        "http-requests",
                        new Message<string, HttpRequest>
                        {
                            Key = "user1@somewhere.io",
                            Value = httpRequest
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(30));
            }
        }
    }
}
