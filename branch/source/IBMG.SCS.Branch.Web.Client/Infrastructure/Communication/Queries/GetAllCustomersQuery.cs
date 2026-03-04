// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/customers/get-all")]
    public record GetAllCustomersQuery() : IQuery<GetAllCustomersQueryResult>;
}