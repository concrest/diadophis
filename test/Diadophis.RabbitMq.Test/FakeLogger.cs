// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;

namespace Diadophis.RabbitMq.Test
{
    [ExcludeFromCodeCoverage]
    internal class FakeLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return Mock.Of<IDisposable>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogEntries.Add(new LogEntry(logLevel, eventId));
        }

        internal class LogEntry
        {
            public LogEntry(LogLevel logLevel, EventId eventId)
            {
                LogLevel = logLevel;
                EventId = eventId;
            }

            public LogLevel LogLevel { get; }
            public EventId EventId { get; }
        }
    }
}
