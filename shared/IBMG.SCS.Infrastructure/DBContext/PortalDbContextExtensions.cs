// Copyright (c) IBMG. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IBMG.SCS.Infrastructure.DBContext
{
    public static class PortalDbContextExtensions
    {
        public static async Task<IServiceProvider> ApplyPortalMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<PortalDBContext>>();

            var ctx = await dbContext.CreateDbContextAsync();

            await ctx.Database.MigrateAsync();

            return serviceProvider;
        }
    }
}