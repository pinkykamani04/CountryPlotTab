// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllTradeCardsQueryResult(IReadOnlyList<CardModel> tradeCards) : IQueryResult
    {
        public IReadOnlyList<CardModel> TradeCards { get; } = tradeCards;
    }
}