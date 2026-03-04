// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Infrastructure.Auditing;

namespace IBMG.SCS.Portal.Web.Infrastructure.Extensions
{
    public static class AuditingExtensions
    {
        public static IServiceCollection AddPortalAuditing(this IServiceCollection services)
        {
            //services.AddScoped<IOperativeChangeProcessor, OperativeAuditingChangeProcessor>();
            //services.AddScoped<ISpendLimitChangeProcessor, SpendLimitAuditingChangeProcessor>();
            //services.AddScoped<ITradeCardChangeProcessor, TradeCardAuditingChangeProcessor>();

            return services;
        }
    }
}