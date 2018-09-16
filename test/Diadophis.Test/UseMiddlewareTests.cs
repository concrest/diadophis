// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Diadophis.Test.Middleware;
using Xunit;

namespace Diadophis.Test
{
    public class UseMiddlewareTests
    {
        private readonly PipelineBuilder _sut;
        private readonly FakeServiceProvider _fakeServiceProvider;
        private readonly FakeMessageContext _fakeMessageContext;

        public UseMiddlewareTests()
        {
            _fakeServiceProvider = new FakeServiceProvider();
            _sut = new PipelineBuilder(_fakeServiceProvider);

            _fakeMessageContext = new FakeMessageContext(_fakeServiceProvider);
        }

        //[Fact]
        //public void UseMiddleware_adds_class_with_default_constructor()
        //{
        //    var pipeline = _sut.UseMiddleware<BasicMiddleware>().Build();

        //}

    }
}
