// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddTradeCardCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddTradeCardCommand
{
    [BluQubeCommand(Path = "commands/tradecard/add")]
    public record AddTradeCardCommand(Guid Id, string TradeCardNumber, Guid? AssigneeId, string Status) : ICommand<AddTradeCardCommandResult>;
}