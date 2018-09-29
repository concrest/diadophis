// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Diadophis.Test
{
    public class PipelineBuilderTests
    {
        private readonly PipelineBuilder _sut;
        private readonly FakeServiceProvider _fakeServiceProvider;
        private readonly FakeMessageContext _fakeMessageContext;

        public PipelineBuilderTests()
        {
            _fakeServiceProvider = new FakeServiceProvider();
            _sut = new PipelineBuilder(_fakeServiceProvider, Mock.Of<ILogger<PipelineBuilder>>());

            _fakeMessageContext = new FakeMessageContext(_fakeServiceProvider);
        }

        [Fact]
        public void Empty_pipeline_returns_completed_task()
        {
            var pipeline = _sut.Build();

            var actual = pipeline.Invoke(_fakeMessageContext);

            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public void Use_adds_middleware_to_pipeline()
        {
            var middlewareCalled = false;
            MessageDelegate middleware(MessageDelegate @delegate) => context =>
            {
                middlewareCalled = true;
                return Task.CompletedTask;
            };

            _sut.Use(middleware);

            _sut.Build().Invoke(_fakeMessageContext);

            Assert.True(middlewareCalled);
        }

        [Fact]
        public void Run_adds_terminal_middleware_to_pipeline()
        {
            _sut.Run(context => Task.CompletedTask);

            // This next middleware shouldn't be run if .Run is really terminal:
            _sut.Run(MiddlewareWithException);

            var actual = _sut.Build().Invoke(_fakeMessageContext);

            Assert.True(actual.IsCompletedSuccessfully);
        }

        [ExcludeFromCodeCoverage]
        private Task MiddlewareWithException(MessageContext context)
        {
            // Will only be called if .Run isn't terminal
            // So this code is not executed if the test passes
            throw new NotSupportedException();
        }
    }
}
