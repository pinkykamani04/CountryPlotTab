// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpdateSpendLimitCommandResult
{
    public record UpdateSpendLimitCommandResult(Guid Id, Status Status, int TnxLimit, int DailyLimit, int WeeklyLimit, int MonthlyLimit, DateTime EndDate) : ICommandResult;

}