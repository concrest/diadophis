// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Inspired by https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.Http/Internal/ApplicationBuilder.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Diadophis
{
    public class PipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<MessageDelegate, MessageDelegate>> _components = new List<Func<MessageDelegate, MessageDelegate>>();
        private readonly ILogger<PipelineBuilder> _logger;

        private class LoggingEvents
        {
            internal static readonly EventId Use = new EventId(101, nameof(Use));
            internal static readonly EventId Build = new EventId(102, nameof(Build));
        }

        public IServiceProvider ApplicationServices { get; }

        public PipelineBuilder(IServiceProvider serviceProvider,
            ILogger<PipelineBuilder> logger)
        {
            ApplicationServices = serviceProvider;
            _logger = logger;
        }

        public MessageDelegate Build()
        {
            _logger.LogDebug(LoggingEvents.Build, "Building pipeline from {ComponentCount} components", _components.Count);

            MessageDelegate app = context =>
            {
                return Task.CompletedTask;
            };

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }

        public IPipelineBuilder Use(Func<MessageDelegate, MessageDelegate> middleware)
        {
            _logger.LogTrace(LoggingEvents.Use, "Adding middleware component");

            _components.Add(middleware);
            return this;
        }
    }
}
