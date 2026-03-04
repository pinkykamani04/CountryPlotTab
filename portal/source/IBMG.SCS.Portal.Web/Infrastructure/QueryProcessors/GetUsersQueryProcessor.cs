// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Data;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Infrastructure.Multitenancy;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Client.Constants;
using PearDrop.Authentication.Contracts;
using System.Security.Claims;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors;

public sealed class GetUsersQueryHandler(
    IAuthReadModels authReadModels, ITenantExecutionScope tenantExecutionScope,
    IHttpContextAccessor httpContextAccessor)
    : IQueryProcessor<GetUsersQuery, GetUsersQueryResult>
{
    public async Task<QueryResult<GetUsersQueryResult>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        // Always use explicit tenant filtering instead of relying on global query filters
        // which have architectural issues with DbContextFactory (see TENANT-ISOLATION-ISSUES.md)
        var userTenantId = GetCurrentUserTenantId(httpContextAccessor.HttpContext);
        Guid? tenantFilter = query.TenantId ?? userTenantId; // Use provided TenantId or fall back to current user's tenant

        // Check if this is a cross-tenant query (TenantId differs from current user's tenant)
        if (query.TenantId.HasValue && userTenantId.HasValue && userTenantId != query.TenantId)
        {
            // Cross-tenant query - validate authorization
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.HasClaim(c => c.Type == ClaimTypes.System))
            {
                throw new UnauthorizedAccessException("Only SystemRoot users can query users in other tenants.");
            }
        }

        if (!tenantFilter.HasValue)
        {
            throw new InvalidOperationException("TenantId is required to query users.");
        }

        using (tenantExecutionScope.Begin(tenantFilter.Value))
        {
            var usersQueryable = authReadModels.Users
                .AsNoTracking()
                .Include(u => u.UserPrincipalNames);

            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var lower = query.SearchText.ToLower();
                usersQueryable = usersQueryable.Where(u =>
                    u.ContactEmailAddress.ToLower().Contains(lower) ||
                    u.UserPrincipalNames.Any(p => p.Value.ToLower().Contains(lower)));
            }

            if (!query.ShowLocked)
            {
                usersQueryable = usersQueryable.Where(u => !u.WhenLocked.HasValue);
            }

            var result = await usersQueryable.Include(x => x.Profile).OrderByDescending(u => u.WhenCreated)
                                             .Select(u => new QueryableUser
                                             {
                                                 Id = u.Id,
                                                 Firstname = u.Profile!.FirstName,
                                                 Lastname = u.Profile!.LastName,
                                                 UserPrincipalName = u.UserPrincipalNames.OrderByDescending(p => p.WhenVerified ?? DateTime.MinValue).Select(p => p.Value).First(),
                                                 IsLocked = u.WhenLocked.HasValue,
                                                 Status = u.UserPrincipalNames.OrderByDescending(p => p.WhenVerified ?? DateTime.MinValue).Select(p => p.Status).First(),
                                                 CreatedDate = u.WhenCreated,
                                             })
                                             .ToListAsync(cancellationToken);

            return QueryResult<GetUsersQueryResult>
                .Succeeded(new GetUsersQueryResult(result));
        }
    }

    private static Guid? GetCurrentUserTenantId(HttpContext? httpContext)
    {
        if (httpContext == null)
            return null;

        var tenantIdClaim = httpContext.User.FindFirst("TenantId");
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return tenantId;
        }

        return null;
    }
}