// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    public class CtorDependencyInjection
    {
        private readonly MessageDelegate _next;
        private readonly IMiddlewareDependency _dependency;

        public CtorDependencyInjection(MessageDelegate next, IMiddlewareDependency dependency)
        {
            _next = next;
            _dependency = dependency;
        }

        public Task InvokeAsync(MessageContext context)
        {
            _dependency.IncrementCallCount();

            return _next(context);
        }
    }
}
