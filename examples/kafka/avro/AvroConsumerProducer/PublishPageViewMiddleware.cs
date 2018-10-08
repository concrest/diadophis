// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Confluent.Kafka;
using Diadophis;
using Diadophis.Kafka;
using Microsoft.Extensions.Logging;
using SchemaExamples.Model;

namespace AvroConsumerProducer
{
    /// <summary>
    /// Publishes a PageView object to Kafka if one is found in the message context
    /// </summary>
    public class PublishPageViewMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly ILogger<PublishPageViewMiddleware> _logger;

        public PublishPageViewMiddleware(MessageDelegate next, 
            ILogger<PublishPageViewMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(MessageContext context)
        {
            var kafkaMessage = context.GetKafkaMessage<string, HttpRequest>();
            var pageView = context.GetPageView();

            if (kafkaMessage != null && pageView != null)
            {
                _logger.LogInformation("Publishing {PageType} for user {User}",
                    pageView.page_type, kafkaMessage.Key);

                // TODO: publish event here
            }

            // We know that in its current state this middleware is the last one in the pipeline,
            // but there's no overhead in passing to the next one, and keeping a consistent pattern
            // makes it easier to add middleware later.
            // General rule of thumb is that middleware shouldn't know if they are the first or last 
            // piece in the pipeline.
            return _next(context);
        }

        private PageView CreatePageView(ConsumeResult<string, HttpRequest> kafkaMessage)
        {
            return new PageView
            {
                timestamp = kafkaMessage.Value.timestamp,
                user_agent = kafkaMessage.Value.user_agent,
                page_type = DerivePageType(kafkaMessage.Value.path)
            };
        }

        private PageTypes DerivePageType(string path)
        {
            // Naive mapping from path to PageType
            // Provided as a simple example only

            if (path.StartsWith("/login"))
            {
                return PageTypes.Login;
            }

            if (path.StartsWith("/logout"))
            {
                return PageTypes.LogOut;
            }

            if (path.StartsWith("/dashboard"))
            {
                return PageTypes.Dashboard;
            }

            if (path.StartsWith("/settings"))
            {
                return PageTypes.Settings;
            }

            if (path.StartsWith("/reports"))
            {
                return PageTypes.Reports;
            }

            return PageTypes.Other;
        }
    }
}
