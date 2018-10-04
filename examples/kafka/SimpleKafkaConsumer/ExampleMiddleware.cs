// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis;
using Diadophis.Kafka;
using Microsoft.Extensions.Logging;

namespace SimpleKafkaConsumer
{
    public class ExampleMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly ILogger<ExampleMiddleware> _logger;

        /// <summary>
        /// Constructor for middleware must have the first argument be a MessageDelegate for the 
        /// next delegate to invoke.
        /// Other arguments will be created using Dependency Injection, so must be registered 
        /// with the IoC container as part of application startup
        /// </summary>
        /// <param name="next">The next delegate in the pipeline</param>
        /// <param name="dependency">A class level dependency, created once per middleware object</param>
        public ExampleMiddleware(MessageDelegate next, ILogger<ExampleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// First argument to the InvokeAsync method must be the MessageContext
        /// being passed along the pipeline.
        /// Othe arguments for this method are where it makes sense to create new objects
        /// per message rather than 1 object created for the constructor injection.
        /// As in constructor dependency inject, dependencies must be wired up with the IoC
        /// container as part of application startup
        /// </summary>
        /// <param name="context">The message context being passed along the pipeline</param>
        /// <param name="invokeDependency">A dependency to instantiate per message rather than per middleware object</param>
        /// <returns></returns>
        public Task InvokeAsync(MessageContext context)
        {
            // You can access the message from Kafka here:
            var kafkaMessage = context.GetKafkaMessage<Ignore, string>();

            _logger.LogDebug("InvokeAsync called in middleware for {Value}", kafkaMessage.Value);

            // Add any processing, ETL, transforms, filtering etc.

            // Optionally call the next step
            return _next(context);
        }
    }
}