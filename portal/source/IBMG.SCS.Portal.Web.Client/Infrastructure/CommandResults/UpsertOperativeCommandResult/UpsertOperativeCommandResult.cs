// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertOperativeCommandResult
{
    public record UpsertOperativeCommandResult(Guid Id, string FirstName, string LastName, Guid JobRole, string OperativeNumber, decimal TnxLimit,
        decimal DailyLimit, decimal WeeklyLimit, decimal MonthlyLimit, DateTime StartDate, DateTime EndDate, Status Status, Guid TradeCardId)
        : ICommandResult;
}