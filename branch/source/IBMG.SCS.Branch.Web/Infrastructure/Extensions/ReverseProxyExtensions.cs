// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Branch.Web.Infrastructure.ReverseProxy;

namespace IBMG.SCS.Branch.Web.Infrastructure.Extensions
{
    public static class ReverseProxyExtensions
    {
        public static IServiceCollection AddKerridgeProxy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                .AddTransforms<KerridgeAuthTransform>();

            services.AddSingleton<KerridgeAuthTransform>();

            return services;
        }
    }
}