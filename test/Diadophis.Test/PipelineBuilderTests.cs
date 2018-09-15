// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Diadophis.Test
{
    public class PipelineBuilderTests
    {
        private readonly PipelineBuilder _sut;

        public PipelineBuilderTests()
        {
            _sut = new PipelineBuilder();
        }

        [Fact]
        public void Empty_pipeline_returns_completed_task()
        {
            var pipeline = _sut.Build();

            var actual = pipeline.Invoke(new FakeMessageContext());

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

            _sut.Build().Invoke(new FakeMessageContext());

            Assert.True(middlewareCalled);
        }

        [Fact]
        public void Run_adds_terminal_middleware_to_pipeline()
        {
            _sut.Run(context => Task.CompletedTask);

            _sut.Run(MiddlewareWithException);

            var actual = _sut.Build().Invoke(new FakeMessageContext());

            Assert.True(actual.IsCompletedSuccessfully);
        }

        [ExcludeFromCodeCoverage]
        private Task MiddlewareWithException(MessageContext context)
        {
            throw new NotSupportedException();
        }
    }
}
