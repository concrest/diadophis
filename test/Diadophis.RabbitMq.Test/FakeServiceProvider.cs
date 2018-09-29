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
