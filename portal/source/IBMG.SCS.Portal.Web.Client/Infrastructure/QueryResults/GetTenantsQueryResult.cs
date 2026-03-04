// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public sealed record GetTenantsQueryResult(IEnumerable<QueryableTenant> Tenants) : IQueryResult;
}