// Copyright 2018 Concrest
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace SimpleRabbitMqService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseMyDependencies(this IServiceCollection services)
        {
            // Wire up middleware dependencies.

            // Singletons are created once - the same object is used for each 
            services.AddSingleton<IConstructorDependency, ConstructorDependency>();

            // Transient services get created whenever they are needed
            services.AddTransient<IInvokeDependency, InvokeDependency>();

            return services;
        }
    }
}
