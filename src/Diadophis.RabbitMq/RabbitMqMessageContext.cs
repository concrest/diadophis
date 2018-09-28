﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using RabbitMQ.Client.Events;

namespace Diadophis.RabbitMq
{
    public class RabbitMqMessageContext : MessageContext
    {
        public RabbitMqMessageContext(IServiceProvider serviceProvider, BasicDeliverEventArgs message)
            : base(serviceProvider)
        {
            this.SetRabbitMqMessage(message);
        }
    }
}