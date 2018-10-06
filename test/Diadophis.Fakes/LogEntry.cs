// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Diadophis.Fakes
{
    public class LogEntry
    {
        public LogEntry(LogLevel logLevel, EventId eventId, IEnumerable<KeyValuePair<string, object>> values)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Values = values;
        }

        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public IEnumerable<KeyValuePair<string, object>> Values { get; }

        public object GetValue(string key) 
            => Values
            .Where(kvp => kvp.Key == key)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();
    }
}
