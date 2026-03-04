// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using IBMG.SCS.Portal.Web.Infrastructure.Multitenancy;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Queries.Entities;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public sealed class GetCustomerUserPrincipalIdQueryProcessor
        : IQueryProcessor<GetCustomerUserPrincipalIdQuery, GetActiveUserPrincipalNameIdQueryResult>
    {
        private readonly IDbContextFactory<PortalDBContext> _dbContextFactory;
        private readonly ITenantExecutionScope _tenantExecutionScope;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCustomerUserPrincipalIdQueryProcessor(
            IDbContextFactory<PortalDBContext> dbContextFactory,
            ITenantExecutionScope tenantExecutionScope,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContextFactory = dbContextFactory;
            _tenantExecutionScope = tenantExecutionScope;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<QueryResult<GetActiveUserPrincipalNameIdQueryResult>> Handle(
            GetCustomerUserPrincipalIdQuery query,
            CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

            var tenantId = GetTenantIdFromClaims(httpContext);

            using (_tenantExecutionScope.Begin(tenantId))
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var user = await db.Set<QueryableUser>()
                    .AsNoTracking()
                    .Include(u => u.UserPrincipalNames)
                    .SingleOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

                var principalId = user?.UserPrincipalNames.FirstOrDefault()?.Id;

                return QueryResult<GetActiveUserPrincipalNameIdQueryResult>
                    .Succeeded(new GetActiveUserPrincipalNameIdQueryResult(principalId));
            }
        }

        private static Guid GetTenantIdFromClaims(HttpContext httpContext)
        {
            var tenantIdClaim = httpContext.User.FindFirst("TenantId");

            if (tenantIdClaim == null ||
                !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
            {
                throw new InvalidOperationException("TenantId claim is missing.");
            }

            return tenantId;
        }
    }
}