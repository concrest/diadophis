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
    /// Consumes a HTTP Request event and works out the PageView
    /// object for it.  This translate incoming HTTP requests to a 
    /// broader PageView object for other middleware in the pipeline
    /// </summary>
    public class MapRequestToPageViewMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly ILogger<MapRequestToPageViewMiddleware> _logger;

        public MapRequestToPageViewMiddleware(MessageDelegate next, 
            ILogger<MapRequestToPageViewMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(MessageContext context)
        {
            var kafkaMessage = context.GetKafkaMessage<string, HttpRequest>();
            var pageView = CreatePageView(kafkaMessage);

            _logger.LogInformation("Mapped request {Verb} {Path} for user {User} to a {PageType}",
                kafkaMessage.Value.request_verb, kafkaMessage.Value.path, kafkaMessage.Key, pageView.page_type);

            context.SetPageView(pageView);

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
