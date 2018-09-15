// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diadophis
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Adds a terminal middleware delegate to the application's message pipeline.
        /// </summary>
        public static IPipelineBuilder Run(this IPipelineBuilder builder, MessageDelegate handler)
        {
            builder.Use(_ => handler);
            return builder;
        }
    }
}
