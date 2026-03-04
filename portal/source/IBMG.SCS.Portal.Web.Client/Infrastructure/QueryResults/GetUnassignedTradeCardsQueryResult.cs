// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetUnassignedTradeCardsQueryResult : IQueryResult
    {
        public IReadOnlyList<CardModel> TradeCards { get; }

        public GetUnassignedTradeCardsQueryResult(IReadOnlyList<CardModel> tradeCards)
        {
            TradeCards = tradeCards;
        }
    }
}
