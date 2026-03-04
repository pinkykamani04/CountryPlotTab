// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.KerridgeApi.Server.KerridgeDwhModels;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.KerridgeApi.Server.AppDbContext
{
    public class IbmgDwhDbContext(DbContextOptions<IbmgDwhDbContext> options) : DbContext(options)
    {
        public DbSet<ScsCustomer> Customers => this.Set<ScsCustomer>();

        public DbSet<ScsSalesAndOrders> SalesAndOrders => this.Set<ScsSalesAndOrders>();

        public DbSet<ScsSalesAndOrdersLines> SalesAndOrdersLines => this.Set<ScsSalesAndOrdersLines>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScsCustomer>(entity =>
            {
                entity.ToView("vw_dimension_customers", "scs");
                entity.HasNoKey();
            });

            modelBuilder.Entity<ScsSalesAndOrdersLines>(entity =>
            {
                entity.ToView("vw_fact_salesandorderlines", "scs");
                entity.HasNoKey();
            });

            modelBuilder.Entity<ScsSalesAndOrders>(entity =>
            {
                entity.ToView("vw_fact_salesandorders", "scs");
                entity.HasNoKey();
            });
        }
    }
}