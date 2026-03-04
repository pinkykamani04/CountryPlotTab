// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetSCSUsersQueryResult(IReadOnlyList<GetSCSUsersQueryResult.UserItem> users) : IQueryResult
    {
        public IReadOnlyList<UserItem> Users { get; } = users;

        public record UserItem(
            Guid Id,
            string FirstName,
            string LastName,
            string Email,
            string Status
        );
    }
}
