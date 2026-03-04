// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using Finbuckle.MultiTenant.Abstractions;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;
using PearDrop.Multitenancy;
using PearDrop.Multitenancy.Contracts;
using PearDrop.Multitenancy.Domain.Tenant;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers
{
    public sealed class GetTenantByIdQueryHandler(
    IMultitenancyReadModels readModels,
    IMultiTenantContextAccessor<PearDropTenantInfo> tenantContextAccessor,
    ICurrentAuthenticatedUserProvider currentUserProvider)
    : IQueryProcessor<GetTenantByIdQuery, QueryableTenant>
    {
        public async Task<QueryResult<QueryableTenant>> Handle(GetTenantByIdQuery query, CancellationToken cancellationToken)
        {
            // Get the current user to check if they're a system admin
            var currentUser = currentUserProvider.CurrentAuthenticatedUser;
            var isSystemAdmin = currentUser.HasValue && currentUser.Value is IUserWithRoles userWithRoles && userWithRoles.IsSystemRoot;

            // If not a system admin, verify the requested tenant is the current tenant
            if (!isSystemAdmin && tenantContextAccessor.MultiTenantContext?.TenantInfo != null)
            {
                var currentTenantId = Guid.Parse(tenantContextAccessor.MultiTenantContext.TenantInfo.Id);
                if (query.TenantId != currentTenantId)
                {
                    // Return empty result if user doesn't have access to this tenant
                    return QueryResult<QueryableTenant>.Succeeded(new QueryableTenant());
                }
            }

            var t = await readModels.Tenants
                .Include(x => x.MetaItems)
                .Where(x => x.Id == query.TenantId)
                .FirstOrDefaultAsync(cancellationToken);
            if (t == null)
            {
                return QueryResult<QueryableTenant>.Succeeded(new QueryableTenant());
            }

            var useDeviceRemembrance = t.MetaItems?.FirstOrDefault(m => m.Key == TenantMetaItemKeys.UseDeviceRemembrance)?.Value == "true";
            var deviceRemembranceExpirationStr = t.MetaItems?.FirstOrDefault(m => m.Key == TenantMetaItemKeys.DeviceRemembranceExpirationInDays)?.Value;
            int? deviceRemembranceExpiration = deviceRemembranceExpirationStr != null && int.TryParse(deviceRemembranceExpirationStr, out var days) ? days : null;

            return QueryResult<QueryableTenant>.Succeeded(new QueryableTenant
            {
                Id = t.Id,
                Identifier = t.Identifier,
                Name = t.Name,
                IsDisabled = t.WhenDisabled.HasValue,
                WhenDisabled = t.WhenDisabled,
                DeviceRemembranceExpirationInDays = deviceRemembranceExpiration,
                UseDeviceRemembrance = useDeviceRemembrance ? true : null
            });
        }
    }
}