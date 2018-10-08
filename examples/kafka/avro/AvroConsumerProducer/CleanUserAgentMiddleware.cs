// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Diadophis;
using Microsoft.Extensions.Logging;
using SchemaExamples.Model;

namespace AvroConsumerProducer
{
    /// <summary>
    /// Sanitises the user agent value on the basic PageView object
    /// </summary>
    public class CleanUserAgentMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly ILogger<CleanUserAgentMiddleware> _logger;

        public CleanUserAgentMiddleware(MessageDelegate next, 
            ILogger<CleanUserAgentMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(MessageContext context)
        {
            var pageView = context.GetPageView();

            // Example usecase here is if we get a lot of bogus user agents that
            // services downstream of this processing need to omit.  This stage
            // replaces the bogus user agent settings with a fixed default so 
            // downstream services don't have to sanitise the data any further
            if (pageView != null && UserAgentIsUnknown(pageView))
            {
                _logger.LogInformation("Replacing bogus user agent {UserAgent}", pageView.user_agent);
                pageView.user_agent = "Unknown";
            }

            return _next(context);
        }

        private bool UserAgentIsUnknown(PageView pageView)
        {
            // Naive pattern matching for example only - real implementation
            // could be a whitelist of user agent patterns
            return pageView.user_agent.Contains("bogus_browser");
        }
    }
}
