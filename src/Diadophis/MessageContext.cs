// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Diadophis
{
    public abstract class MessageContext
    {
        public IServiceProvider MessageServices { get; }

        protected MessageContext(IServiceProvider serviceProvider)
        {
            MessageServices = serviceProvider;
        }
    }
}