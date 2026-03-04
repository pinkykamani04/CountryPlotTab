// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertOperativeCommandResult;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertOperativeCommand
{
    [BluQubeCommand(Path = "commands/operative/add")]
    public record UpsertOperativeCommand(Guid Id, string FirstName, string LastName, Guid JobRole, string OperativeNumber,
                                         decimal TnxLimit, decimal DailyLimit, decimal WeeklyLimit, decimal MonthlyLimit,
                                         DateTime StartDate, DateTime EndDate, Status Status, Guid TradeCardId)
       : ICommand<UpsertOperativeCommandResult>;
}