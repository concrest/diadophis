﻿// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Diadophis
{
    public interface IPipelineBuilder
    {
        MessageDelegate Build();
        PipelineBuilder Use(Func<MessageDelegate, MessageDelegate> middleware);
    }
}