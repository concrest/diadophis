// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    [ExcludeFromCodeCoverage]
    public class InvokeMethodWithRefArg
    {
        public InvokeMethodWithRefArg(MessageDelegate next)
        {
        }

        public Task InvokeAsync(MessageContext context, ref bool value)
        {
            return Task.CompletedTask;
        }
    }
}