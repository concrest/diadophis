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
        [InlineData(typeof(CtorDependencyInjection))]
        [InlineData(typeof(InvokeAsyncMethodInjection))]
        public void UseMiddleware_supports_dependency_injection(Type middlewareType)
        {
            var middlewareDependency = new MiddlewareDependency();

            _fakeServiceProvider.Register<IMiddlewareDependency>(middlewareDependency);

            _sut
               .UseMiddleware(middlewareType)
               .Build()
               .Invoke(_fakeMessageContext);

            Assert.Equal(1, middlewareDependency.CallCount);
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
        [InlineData(typeof(InvokeMethodWithRefArg),
            "The 'InvokeAsync' method must not have ref or out parameters.")]
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

        [Fact]
        public void UseMiddleware_throws_on_Invoke_for_unknown_dependencies()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _sut
                    .UseMiddleware<InvokeAsyncMethodInjection>()
                    .Build()
                    .Invoke(_fakeMessageContext);
            });

            Assert.Equal("Unable to resolve service for type 'Diadophis.Test.IMiddlewareDependency' while attempting to Invoke middleware 'Diadophis.Test.Middleware.InvokeAsyncMethodInjection'.", 
                exception.Message);
        }

        [Fact]
        public void UseMiddleware_throws_on_Invoke_when_service_provider_unavailable()
        {
            var sutWithNoIoC = new PipelineBuilder(null);
            var contextWithNoIoC = new FakeMessageContext(null);

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                sutWithNoIoC
                    .UseMiddleware<InvokeAsyncMethodInjection>()
                    .Build()
                    .Invoke(contextWithNoIoC);
            });

            Assert.Equal("'IServiceProvider' is not available.", exception.Message);
        }
    }
}
