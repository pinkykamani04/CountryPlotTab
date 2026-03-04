// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Infrastructure.ReverseProxy;
using Yarp.ReverseProxy.Transforms.Builder;

namespace IBMG.SCS.Portal.Web.Infrastructure.Extensions;

public static class ReverseProxyExtensions
{
    public static IServiceCollection AddKerridgeProxy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddReverseProxy().LoadFromConfig(configuration.GetSection("ReverseProxy")).AddTransforms<KerridgeAuthTransform>();
        services.AddSingleton<KerridgeAuthTransform>();
        services.AddSingleton<ITransformProvider, KerridgeAuthTransform>();

        return services;
    }
}