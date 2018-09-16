// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Diadophis.Test
{
    internal class FakeServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _simpleContainer = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            return _simpleContainer.ContainsKey(serviceType) ?
                _simpleContainer[serviceType] : null;
        }

        public void Register<TType>(TType impl)
        {
            _simpleContainer.Add(typeof(TType), impl);
        }
    }
}