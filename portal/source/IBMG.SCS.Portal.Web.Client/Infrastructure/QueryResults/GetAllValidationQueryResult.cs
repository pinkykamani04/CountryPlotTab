// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllValidationQueryResult(IReadOnlyList<ValidationDto> tradeCards) : IQueryResult
    {
        public IReadOnlyList<ValidationDto> TradeCards { get; } = tradeCards;
    }
}