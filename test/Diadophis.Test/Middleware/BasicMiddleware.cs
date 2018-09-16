// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    public class BasicMiddleware
    {
        public const string CountPropertyKey = "BasicMiddlewareInvokeCount";

        private readonly MessageDelegate _next;

        public BasicMiddleware(MessageDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(MessageContext context)
        {
            var currentInvokeCount = context.GetProperty<int>(CountPropertyKey);
            currentInvokeCount++;

            context.SetProperty(CountPropertyKey, currentInvokeCount);


            return _next(context);
        }
    }
}
