using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PearDrop.Authentication.Data;
using PearDrop.Database.Contracts;
using PearDrop.Domain.Contracts;
using PearDrop.Multitenancy;
using PearDrop.Settings;

namespace IBMG.SCS.Infrastructure.DBContext
{
    public class PortalDbContextFactory : IDesignTimeDbContextFactory<PortalDBContext>
    {
        private const string StaticConnectionString = "Server=DELL;Database=AnalyticsData;Trusted_Connection=True;TrustServerCertificate=True";

        public PortalDBContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Setup DI for design-time dependencies
            var services = new ServiceCollection();

            // Configure settings
            services.Configure<CoreSettings>(opt => configuration.GetSection("PearDrop:modules:core").Bind(opt));

            // Register single-tenant context accessor
            services.AddSingleton<IMultiTenantContextAccessor<PearDropTenantInfo>, SingleTenantContextAccessor>();
            services.AddSingleton<IMultiTenantContextAccessor>(sp => sp.GetRequiredService<IMultiTenantContextAccessor<PearDropTenantInfo>>());

            // Register empty collections for design-time dependencies
            services.AddSingleton<IEnumerable<IEntityFilterProvider>>(new List<IEntityFilterProvider>());
            services.AddSingleton<IEnumerable<IChangeProcessor<PortalDBContext>>>(new List<IChangeProcessor<PortalDBContext>>());
            services.AddSingleton<IEnumerable<IPersistenceModifier>>(new List<IPersistenceModifier>());

            var serviceProvider = services.BuildServiceProvider();

            // Get connection string
            var coreSettings = serviceProvider.GetRequiredService<IOptions<CoreSettings>>().Value;
            var connectionString = coreSettings.PrimaryConnectionString ?? StaticConnectionString;

            // Build DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<PortalDBContext>();

            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.MigrationsAssembly("IBMG.SCS.Infrastructure");
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            });
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.MigrationsAssembly("IBMG.SCS.Infrastructure");
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            });
            return new PortalDBContext(optionsBuilder.Options);
        }
    }
}