using System;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Diadophis.Kafka
{
    public static class LoggerExtensions
    {
        public static void LogStart<TKey, TValue>(this ILogger logger, EventId eventId, IKafkaConfig<TKey, TValue> config)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            logger.LogInformation(eventId,
                "Starting KafkaConsumerService for BrokerUrls {BrokerUrls}, ConsumerGroupId {ConsumerGroupId}, Topics {Topics}",
                config.BrokerUrls, config.ConsumerGroupId, config.Topics);
        }

        public static void LogKafkaMessage<TKey, TValue>(this ILogger logger, 
            EventId eventId, 
            string prefix,
            ConsumeResult<TKey, TValue> message)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            logger.LogDebug(eventId,
                prefix + ". Topic: {Topic}, Key: {Key}, Value: {Value}, Offset: {Offset}",
                message.Topic,
                message.Key,
                message.Value,
                message.Offset);
        }

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

        public static void LogKafkaException(this ILogger logger, EventId eventId, KafkaException kex)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var e = kex.Error;

            logger.LogError(
                eventId,
                kex,
                "ConsumeException. Code: {Code}, IsBrokerError {IsBrokerError}, IsError: {IsError}, IsLocalError: {IsLocalError}, Reason: {Reason}",
                e.Code,
                e.IsBrokerError,
                e.IsError,
                e.IsLocalError,
                e.Reason);
        }
    }
}
