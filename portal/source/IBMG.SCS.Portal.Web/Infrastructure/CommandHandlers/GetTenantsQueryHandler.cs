// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using Finbuckle.MultiTenant.Abstractions;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;
using PearDrop.Multitenancy;
using PearDrop.Multitenancy.Contracts;
using PearDrop.Multitenancy.Domain.Tenant;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers
{
    public sealed class GetTenantsQueryHandler(
    IMultitenancyReadModels readModels,
    IMultiTenantContextAccessor<PearDropTenantInfo> tenantContextAccessor,
    ICurrentAuthenticatedUserProvider currentUserProvider)
    : IQueryProcessor<GetTenantsQuery, GetTenantsQueryResult>
    {
        public async Task<QueryResult<GetTenantsQueryResult>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
        {
            // Get the current user to check if they're a system admin
            var currentUser = currentUserProvider.CurrentAuthenticatedUser;
            var isSystemAdmin = currentUser.HasValue && currentUser.Value is IUserWithRoles userWithRoles && userWithRoles.IsSystemRoot;

            var tenantsQueryable = readModels.Tenants.Include(t => t.MetaItems);

            // If not a system admin, filter to only show the current tenant
            if (!isSystemAdmin && tenantContextAccessor.MultiTenantContext?.TenantInfo != null)
            {
                var currentTenantId = Guid.Parse(tenantContextAccessor.MultiTenantContext.TenantInfo.Id);
                tenantsQueryable = tenantsQueryable.Where(t => t.Id == currentTenantId);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                var lower = query.SearchText.ToLower();
                tenantsQueryable = tenantsQueryable.Where(t => t.Name.ToLower().Contains(lower) || t.Identifier.ToLower().Contains(lower));
            }

            var tenants = await tenantsQueryable
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);

            var list = tenants.Select(t =>
            {
                var useDeviceRemembrance = t.MetaItems?.FirstOrDefault(m => m.Key == TenantMetaItemKeys.UseDeviceRemembrance)?.Value == "true";
                var deviceRemembranceExpirationStr = t.MetaItems?.FirstOrDefault(m => m.Key == TenantMetaItemKeys.DeviceRemembranceExpirationInDays)?.Value;
                int? deviceRemembranceExpiration = deviceRemembranceExpirationStr != null && int.TryParse(deviceRemembranceExpirationStr, out var days) ? days : null;

                return new QueryableTenant
                {
                    Id = t.Id,
                    Identifier = t.Identifier,
                    Name = t.Name,
                    IsDisabled = t.WhenDisabled.HasValue,
                    WhenDisabled = t.WhenDisabled,
                    DeviceRemembranceExpirationInDays = deviceRemembranceExpiration,
                    UseDeviceRemembrance = useDeviceRemembrance ? true : null,
                };
            }).ToList();

            return QueryResult<GetTenantsQueryResult>.Succeeded(new GetTenantsQueryResult(list));
        }
    }
}