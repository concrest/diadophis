﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Diadophis.Test.Middleware
{
    [ExcludeFromCodeCoverage]
    public class TooManyInvokeMethods
    {

        public Task InvokeAsync(MessageContext context)
        {
            return Task.CompletedTask;
        }

        public void InvokeAsync()
        {
        }
    }
}
