// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddTradeCardCommandResult
{
    public record AddTradeCardCommandResult(Guid Id, string TradeCardNumber, Guid? AssigneeId, string Status) : ICommandResult;

}