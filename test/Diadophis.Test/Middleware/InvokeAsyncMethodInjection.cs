// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    public class InvokeAsyncMethodInjection
    {
        private readonly MessageDelegate _next;

        public InvokeAsyncMethodInjection(MessageDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(MessageContext context, IMiddlewareDependency dependency)
        {
            dependency.IncrementCallCount();

            return _next(context);
        }
    }
}

