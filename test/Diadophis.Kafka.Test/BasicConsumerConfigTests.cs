// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Diadophis.Kafka.Test
{
    public class BasicConsumerConfigTests
    {
        [Fact]
        public void Creates_a_basic_consumer_strategy()
        {
            // Create a fake one for testing
            var sut = new FakeBasicConsumerConfig();

            var actual = sut.CreateConsumerStrategy();

            Assert.IsType<BasicConsumerStrategy<string, string>>(actual);
        }
    }
}
