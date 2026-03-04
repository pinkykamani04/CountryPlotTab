// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Data.EntityTypeConfigurationProviders.QueryableTypeConfiguration;
using PearDrop.Authentication.Queries.Entities;
using PearDrop.Database;
using static PearDrop.Authentication.Client.Domain.User.Commands.CreateUserWithPasswordCommand;

namespace IBMG.SCS.Infrastructure.DBContext
{
    public class PortalDBContext : DbContext
    {
        public PortalDBContext(DbContextOptions<PortalDBContext> options)
            : base(options)
        {
            // EF Design-time resolver to avoid missing Kerridge.Instance1.dll
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                if (args.Name.Contains("Kerridge.Instance1"))
                {
                    return null; // tell EF to ignore this DLL
                }

                return null;
            };
        }

        public DbSet<DashboardItem> DashboardItems => Set<DashboardItem>();

        public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();

        public DbSet<Widget> Widgets => Set<Widget>();

        public DbSet<TradeCards> TradeCards => Set<TradeCards>();

        public DbSet<Operatives> Operatives => Set<Operatives>();

        public DbSet<AuditLogs> AuditLogs => Set<AuditLogs>();

        public DbSet<JobRoles> JobRoles => Set<JobRoles>();

        public DbSet<Emails> Emails => Set<Emails>();

        public DbSet<Validation> Validations => Set<Validation>();

        public DbSet<BranchKerridgeMap> BranchKerridgeMaps => this.Set<BranchKerridgeMap>();
        public DbSet<CustomerSite> CustomerSites { get; set; }
        public DbSet<WasteStream> WasteStreams { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }
        public DbSet<ReportingTransaction> ReportingTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dashboard
            modelBuilder.Entity<DashboardItem>(entity =>
            {
                entity.ToTable("dashboard");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.TemplateType).IsRequired();
            });

            // Widgets
            modelBuilder.Entity<Widget>(entity =>
            {
                entity.ToTable("widgets");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.IsMandatory).IsRequired();
                entity.Property(x => x.Icon).HasMaxLength(100);
            });

            // DashboardWidgets
            modelBuilder.Entity<DashboardWidget>(entity =>
            {
                entity.ToTable("dashboard_widgets");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.RowOrder).IsRequired();
                entity.Property(x => x.Position).IsRequired();
                entity.Property(x => x.RowLayoutType).HasMaxLength(50);

                entity.HasOne(dw => dw.Dashboard)
                      .WithMany(d => d.Widgets)
                      .HasForeignKey(dw => dw.DashboardId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dw => dw.Widget)
                      .WithMany(w => w.DashboardWidgets)
                      .HasForeignKey(dw => dw.WidgetId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

          
            // Apply queryable (read model) configurations using standard EF
            modelBuilder.ApplyConfiguration(new RoleQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssignedResourceQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleToGroupMappingQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticatorAppQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticatorDeviceQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordHistoryQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalUserQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserPrincipalNameQueryableTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaItemQueryableTypeConfiguration());
        }
    }
}