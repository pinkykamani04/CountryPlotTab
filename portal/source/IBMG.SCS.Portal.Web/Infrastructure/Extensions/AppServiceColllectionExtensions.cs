using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using PearDrop.Constants;
using PearDrop.Settings;
using PearDrop.SqlServer.Contracts;

namespace IBMG.SCS.Portal.Web.Infrastructure.Extensions
{
    public static class AppServiceColllectionExtensions
    {
        public static IServiceCollection RegisterApplicationDbContextFactory<TPearDbContext>(this IServiceCollection services, string migrationsAssembly)
            where TPearDbContext : DbContext
        {
            if (string.IsNullOrEmpty(migrationsAssembly))
            {
                throw new ArgumentNullException(nameof(migrationsAssembly));
            }

            services.AddDbContextFactory<TPearDbContext>(
                (provider, builder) =>
                {
                    builder.UseExceptionProcessor();

                    builder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

                    var dataSettings = provider.GetRequiredService<IOptions<CoreSettings>>();

                    var additionals = provider.GetServices<IAdditionalSqlServerDbContextOptionsBuilderProvider>();

                    builder.UseSqlServer(
                        dataSettings.Value.PrimaryConnectionString,
                        options =>
                        {
                            options.EnableRetryOnFailure();
                            options.MigrationsAssembly(migrationsAssembly);
                            options.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                            foreach (var additional in additionals)
                            {
                                additional.Apply(options);
                            }
                        });

                    if (dataSettings.Value.SqlLoggingType != SqlLoggingType.None)
                    {
                        builder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
                        if (dataSettings.Value.SqlLoggingType == SqlLoggingType.QueryWithData)
                        {
                            builder.EnableSensitiveDataLogging();
                        }
                    }
                }, ServiceLifetime.Scoped);

            return services;
        }
    }
}