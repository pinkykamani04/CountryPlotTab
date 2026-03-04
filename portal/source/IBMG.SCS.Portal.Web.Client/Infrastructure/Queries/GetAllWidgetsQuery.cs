// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/widgets/get-all")]
    public record GetAllWidgetsQuery : IQuery<GetAllWidgetsQueryResult>;
}