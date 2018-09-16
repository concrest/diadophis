// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis.Test
{
    public class MiddlewareDependency : IMiddlewareDependency
    {
        public int CallCount { get; private set; }

        public void IncrementCallCount()
        {
            CallCount++;
        }
    }
}