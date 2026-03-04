// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddPortalApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddScoped<ICommander, Commander>();
            services.AddScoped<IQuerier, Querier>();

            services.AddScoped<IDashboardService, DashboardService>();
            return services;
        }
    }
}