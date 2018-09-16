// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

        [Fact]
        public void UseMiddleware_adds_class_with_default_constructor()
        {
            _sut
                .UseMiddleware<BasicMiddleware>()
                .Build()
                .Invoke(_fakeMessageContext);

            var middlewareCallCount = _fakeMessageContext.GetProperty<int>(BasicMiddleware.CountPropertyKey);
            Assert.Equal(1, middlewareCallCount);
        }

        [Theory]
        [InlineData(typeof(NoInvokeMethods),
            "No InvokeAsync method found on Diadophis.Test.Middleware.NoInvokeMethods")]
        [InlineData(typeof(TooManyInvokeMethods),
            "Found multiple InvokeAsync methods on Diadophis.Test.Middleware.TooManyInvokeMethods")]
        [InlineData(typeof(InvokeMethodNotReturningTask),
            "InvokeAsync method on Diadophis.Test.Middleware.InvokeMethodNotReturningTask does not return a Task")]
        [InlineData(typeof(InvokeMethodNoArgs), 
            "First parameter on Diadophis.Test.Middleware.InvokeMethodNoArgs.InvokeAsync must be a MessageContext")]
        [InlineData(typeof(InvokeMethodFirstArgNotMessageContext),
            "First parameter on Diadophis.Test.Middleware.InvokeMethodFirstArgNotMessageContext.InvokeAsync must be a MessageContext")]
        public void UseMiddleware_throws_for_invalid_middleware(Type middleware, string expectedError)
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _sut
                    .UseMiddleware(middleware)
                    .Build();
            });

            Assert.Equal(expectedError, exception.Message);
        }
    }
}
