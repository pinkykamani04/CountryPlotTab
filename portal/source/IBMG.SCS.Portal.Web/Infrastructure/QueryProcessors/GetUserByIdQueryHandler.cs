// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using Finbuckle.MultiTenant.Abstractions;
using IBMG.SCS.Portal.Web.Client.Data;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Data;
using PearDrop.Multitenancy;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public sealed class GetUserByIdQueryHandler(
    IDbContextFactory<AuthDbContext> dbContextFactory,
    IMultiTenantContextAccessor<PearDropTenantInfo> tenantContextAccessor)
    : IQueryProcessor<GetUserByIdQuery, QueryableUser>
    {
        public async Task<QueryResult<QueryableUser>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var u = await db.Set<PearDrop.Authentication.Queries.Entities.QueryableUser>()
                .Include(x => x.UserPrincipalNames)
                .FirstOrDefaultAsync(x => x.Id == query.UserId, cancellationToken);

            if (u == null)
            {
                return QueryResult<QueryableUser>.Succeeded(new QueryableUser());
            }

            return QueryResult<QueryableUser>.Succeeded(new QueryableUser
            {
                Id = u.Id,
                UserPrincipalName = u.UserPrincipalNames.FirstOrDefault()?.Value ?? u.ContactEmailAddress,
                IsLocked = u.WhenLocked.HasValue,
                IsDisabled = u.IsDisabled,
                CreatedDate = u.WhenCreated
            });
        }
    }
}