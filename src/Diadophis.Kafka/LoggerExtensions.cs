using System;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Diadophis.Kafka
{
    public static class LoggerExtensions
    {
        public static void LogErrorEvent(this ILogger logger, EventId eventId, ErrorEvent e)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            logger.LogError(
                eventId,
                "ErrorEvent. Code: {Code}, IsBrokerError {IsBrokerError}, IsError: {IsError}, IsFatal: {IsFatal}, IsLocalError: {IsLocalError}, Reason: {Reason}",
                e.Code,
                e.IsBrokerError,
                e.IsError,
                e.IsFatal,
                e.IsLocalError,
                e.Reason);
        }
    }
}
