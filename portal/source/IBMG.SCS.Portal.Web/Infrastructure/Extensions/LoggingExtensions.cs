// Copyright (c) IBMG. All rights reserved.

using Serilog;

namespace IBMG.SCS.Portal.Web.Infrastructure.Extensions
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddPortalLogging(this IServiceCollection services, IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton<Serilog.ILogger>(Log.Logger);

            return services;
        }
    }
}