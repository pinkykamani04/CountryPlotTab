// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddSpendLimitCommandResult
{
    public record AddSpendLimitCommandResult(Guid Id, string CardNumber, string FirstName, string LastName, Status Status,
        decimal TnxLimit, decimal DailyLimit, decimal WeeklyLimit, decimal MonthlyLimit, DateTime EndDate) : ICommandResult;
}