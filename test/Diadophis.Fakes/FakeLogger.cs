// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Diadophis.Fakes
{
    [ExcludeFromCodeCoverage]
    public partial class FakeLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return new FakeDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var values = state as IEnumerable<KeyValuePair<string, object>> ?? new KeyValuePair<string, object>[0];

            LogEntries.Add(new LogEntry(logLevel, eventId, values));
        }
    }
}
