// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public record GetAllCustomersQueryResult(IReadOnlyList<GetAllCustomersQueryResult.CustomerItemDto> Customers) : IQueryResult
    {
        public record CustomerItemDto(Guid CustomerId, string CustomerName);
    }
}