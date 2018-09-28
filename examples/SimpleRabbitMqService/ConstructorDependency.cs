// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace SimpleRabbitMqService
{
    public interface IConstructorDependency
    {
        void RunSomeMethod(int count);
    }

    /// <summary>
    /// Example of a dependency created once for the ExampleMiddleware
    /// constructor injection.  Middleware objects are created once per pipeline.
    /// Note that we can use constructor injection on these dependencies too
    /// </summary>
    public class ConstructorDependency : IConstructorDependency
    {
        private readonly ILogger<ConstructorDependency> _logger;
        private readonly Guid _objectId = Guid.NewGuid();

        public ConstructorDependency(ILogger<ConstructorDependency> logger)
        {
            _logger = logger;
        }

        public void RunSomeMethod(int count)
        {
            _logger.LogInformation("RunSomeMethod called for object {ObjectId}, with value {count}", 
                _objectId, count);
        }
    }
}