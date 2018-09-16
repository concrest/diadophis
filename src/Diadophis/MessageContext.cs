// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Diadophis
{
    public abstract class MessageContext
    {
        public IServiceProvider MessageServices { get; }
        public IDictionary<string, object> Properties { get; }

        protected MessageContext(IServiceProvider serviceProvider)
        {
            MessageServices = serviceProvider;

            Properties = new Dictionary<string, object>();
        }

        public T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out object value) ? (T)value : default(T);
        }

        public void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }
    }
}