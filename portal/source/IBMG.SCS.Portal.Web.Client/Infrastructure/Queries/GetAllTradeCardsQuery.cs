// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/tradecards/get-all")]
    public record GetAllTradeCardsQuery : IQuery<GetAllTradeCardsQueryResult>;
}