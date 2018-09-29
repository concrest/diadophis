// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    public static class MessageContextExtensions
    {
        private static readonly string MessagePropertyName = typeof(BasicDeliverEventArgs).FullName;
        private static readonly string ChannelPropertyName = typeof(IModel).FullName;

        public static void SetRabbitMqMessage(this MessageContext context, BasicDeliverEventArgs message)
        {
            context.SetProperty(MessagePropertyName, message);
        }

        public static BasicDeliverEventArgs GetRabbitMqMessage(this MessageContext context)
        {
            return context.GetProperty<BasicDeliverEventArgs>(MessagePropertyName);
        }

        public static void SetRabbitMqChannel(this MessageContext context, IModel channel)
        {
            context.SetProperty(ChannelPropertyName, channel);
        }

        public static IModel GetRabbitMqChannel(this MessageContext context)
        {
            return context.GetProperty<IModel>(ChannelPropertyName);
        }
    }
}
