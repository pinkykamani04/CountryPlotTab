// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "api/customer-user/id")]
    public record GetCustomerUserPrincipalIdQuery(Guid UserId, Guid? TenantId) : IQuery<GetActiveUserPrincipalNameIdQueryResult>;
}