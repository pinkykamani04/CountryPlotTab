// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/trade/validate")]
    public record ValidateTradeCardQuery(string Customer, string? JobNumber, string? TradeCardNumber, string? OperativeCode, string BranchCode)
        : IQuery<ValidateTradeCardQueryResult>;
}