// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Data;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/users/get-all")]
    public record GetSCSUsersQuery(string? Search) : IQuery<GetSCSUsersQueryResult>;

    public sealed record GetUsersQueryResult(IEnumerable<QueryableUser> Users) : IQueryResult;

    [BluQubeQuery(Path = "api/auth/users")]
    public sealed record GetUsersQuery(string? SearchText, bool ShowLocked, Guid? TenantId = null) : IQuery<GetUsersQueryResult>;

    [BluQubeQuery(Path = "api/auth/users/{UserId}")]
    public sealed record GetUserByIdQuery(Guid UserId) : IQuery<QueryableUser>;
}