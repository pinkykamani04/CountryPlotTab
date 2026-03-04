// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "api/multitenancy/get-tenant-by-id")]
    public sealed record GetTenantByIdQuery(Guid TenantId) : IQuery<QueryableTenant>;
}