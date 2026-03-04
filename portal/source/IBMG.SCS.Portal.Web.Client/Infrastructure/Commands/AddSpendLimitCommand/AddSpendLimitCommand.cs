// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddSpendLimitCommandResult;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddSpendLimitCommand
{
    [BluQubeCommand(Path = "commands/spendlimit/add")]
    public record AddSpendLimitCommand(Guid Id, string CardNumber, string FirstName, string LastName, Status Status,
                                       decimal TnxLimit, decimal DailyLimit, decimal WeeklyLimit, decimal MonthlyLimit, DateTime EndDate,
                                       decimal? OverrideTnxLimit, decimal? OverrideDailyLimit, decimal? OverrideWeeklyLimit,
                                       decimal? OverrideMonthlyLimit, DateTime? OverrideEndDate)
        : ICommand<AddSpendLimitCommandResult>;
}