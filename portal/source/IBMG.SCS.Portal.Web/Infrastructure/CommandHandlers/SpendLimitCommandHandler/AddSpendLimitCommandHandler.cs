// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddSpendLimitCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddSpendLimitCommand;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.SpendLimitCommandHandler
{
    public class AddSpendLimitCommandHandler : CommandHandler<AddSpendLimitCommand, AddSpendLimitCommandResult>
    {
        private readonly PortalDBContext _context;

        public AddSpendLimitCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<AddSpendLimitCommand>> validators,
            ILogger<AddSpendLimitCommandHandler> logger)
            : base(validators, logger)
        {
            this._context = context;
        }

        protected override async Task<CommandResult<AddSpendLimitCommandResult>> HandleInternal(
            AddSpendLimitCommand request,
            CancellationToken cancellationToken)
        {
            var spendLimit = await this._context.Operatives.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (spendLimit == null)
            {
                return CommandResult<AddSpendLimitCommandResult>.Failed(new BluQubeErrorData("Spend Limits Not found"));
            }
            else
            {
                spendLimit.Status = (SCS.Infrastructure.Entities.Status)request.Status;
                spendLimit.TnxLimit = request.TnxLimit;
                spendLimit.DailyLimit = request.DailyLimit;
                spendLimit.WeeklyLimit = request.WeeklyLimit;
                spendLimit.MonthlyLimit = request.MonthlyLimit;
                spendLimit.EndDate = request.EndDate;

                if (request.OverrideTnxLimit.HasValue && request.OverrideDailyLimit.HasValue &&
                    request.OverrideWeeklyLimit.HasValue && request.OverrideMonthlyLimit.HasValue && request.OverrideEndDate.HasValue)
                {
                    spendLimit.OverrideTnxLimit = request.OverrideTnxLimit.Value;
                    spendLimit.OverrideDailyLimit = request.OverrideDailyLimit.Value;
                    spendLimit.OverrideWeeklyLimit = request.OverrideWeeklyLimit.Value;
                    spendLimit.OverrideMonthlyLimit = request.OverrideMonthlyLimit.Value;
                    spendLimit.OverrideEndDate = request.OverrideEndDate;
                }
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<AddSpendLimitCommandResult>.Succeeded(
                new AddSpendLimitCommandResult(
                    spendLimit.Id,
                    request.CardNumber,
                    spendLimit.FirstName,
                    spendLimit.LastName,
                    (Client.Models.Status)spendLimit.Status,
                    spendLimit.TnxLimit,
                    spendLimit.DailyLimit,
                    spendLimit.WeeklyLimit,
                    spendLimit.MonthlyLimit,
                    spendLimit.EndDate));
        }
    }
}