// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace SimpleRabbitMqService
{
    public interface IInvokeDependency
    {
        int GetSomeValue();
    }

    /// <summary>
    /// Example of a dependency to instantiate per invoke call.
    /// One of these objects will be created for each call to InvokeAsync
    /// in the ExampleMiddleware.
    /// Note that we can use constructor injection on these dependencies too
    /// </summary>
    public class InvokeDependency : IInvokeDependency
    {
        private readonly ILogger<InvokeDependency> _logger;
        private readonly Guid _objectId = Guid.NewGuid();

        public InvokeDependency(ILogger<InvokeDependency> logger)
        {
            _logger = logger;
        }

        public int GetSomeValue()
        {
            _logger.LogDebug("GetSomeValue called for object {ObjectId}", _objectId);
            return 123;
        }
    }

   
}