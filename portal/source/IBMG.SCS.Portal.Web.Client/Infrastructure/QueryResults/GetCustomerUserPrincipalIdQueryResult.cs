// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public sealed record GetActiveUserPrincipalNameIdQueryResult(Guid? UserPrincipalNameId) : IQueryResult;
}