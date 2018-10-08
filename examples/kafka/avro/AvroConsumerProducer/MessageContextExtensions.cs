// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Diadophis;
using SchemaExamples.Model;

namespace AvroConsumerProducer
{
    public static class MessageContextExtensions
    {
        private static readonly string PageViewPropertyName = typeof(PageView).FullName;

        public static void SetPageView(this MessageContext context, PageView pageView)
        {
            context.SetProperty(PageViewPropertyName, pageView);
        }

        public static PageView GetPageView(this MessageContext context)
        {
            return context.GetProperty<PageView>(PageViewPropertyName);
        }
    }
}
