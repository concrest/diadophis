// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Confluent.Kafka;

namespace Diadophis.Kafka
{
    public static class MessageContextExtensions
    {
        private static string GetMessagePropertyName<TKey, TValue>() => typeof(ConsumeResult<TKey, TValue>).FullName;

        public static void SetKafkaMessage<TKey, TValue>(this MessageContext context, ConsumeResult<TKey, TValue> consumeResult)
        {
            context.SetProperty(GetMessagePropertyName<TKey, TValue>(), consumeResult);
        }

        public static ConsumeResult<TKey, TValue> GetKafkaMessage<TKey, TValue>(this MessageContext context)
        {
            return context.GetProperty<ConsumeResult<TKey, TValue>>(GetMessagePropertyName<TKey, TValue>());
        }
    }
}
