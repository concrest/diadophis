// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    public class BasicMiddleware
    {
        private readonly MessageDelegate _next;

        public int InvokeCount { get; private set; }

        public BasicMiddleware(MessageDelegate next)
        {
            _next = next;

            InvokeCount = 0;
        }

        public Task InvokeAsync(MessageContext context)
        {
            InvokeCount++;
            return _next(context);
        }
    }
}
