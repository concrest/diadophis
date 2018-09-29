// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Diadophis.RabbitMq.Test
{
    internal class FakeServiceProvider : IServiceProvider
    {
        public Dictionary<Type, object> Container { get; } = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            if (Container.ContainsKey(serviceType))
                return Container[serviceType];

            return null;
        }
    }
}
