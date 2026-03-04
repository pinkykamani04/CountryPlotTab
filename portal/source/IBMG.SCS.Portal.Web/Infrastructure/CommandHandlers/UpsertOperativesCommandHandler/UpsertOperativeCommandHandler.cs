// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertOperativeCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertOperativeCommand;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.UpsertOperativesCommandHandler
{
    public class UpsertOperativeCommandHandler
      : CommandHandler<UpsertOperativeCommand, UpsertOperativeCommandResult>
    {
        private readonly PortalDBContext _context;
        //private readonly IOperativeChangeProcessor _changeProcessor;

        public UpsertOperativeCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<UpsertOperativeCommand>> validators,
            ILogger<UpsertOperativeCommandHandler> logger
            //IOperativeChangeProcessor changeProcessor
            )
            : base(validators, logger)
        {
            this._context = context;
            //this._changeProcessor = changeProcessor;
        }

        protected override async Task<CommandResult<UpsertOperativeCommandResult>> HandleInternal(UpsertOperativeCommand request, CancellationToken cancellationToken)
        {
            var operatives = await this._context.Operatives.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            var oldCards = await this._context.TradeCards.Where(c => c.AssigneeId == request.Id).ToListAsync(cancellationToken);

            foreach (var card in oldCards)
            {
                card.AssigneeId = null;
                card.ModifiedOn = DateTime.UtcNow;
                card.ModifiedBy = "System";
            }

            if (request.TradeCardId != Guid.Empty)
            {
                var newCard = await this._context.TradeCards.FirstOrDefaultAsync(c => c.Id == request.TradeCardId, cancellationToken);

                if (newCard != null)
                {
                    newCard.AssigneeId = request.Id;
                    newCard.ModifiedOn = DateTime.UtcNow;
                    newCard.ModifiedBy = "System";
                }
            }

            if (operatives == null)
            {
                operatives = new Operatives
                {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    JobRole = request.JobRole,
                    OperativeNumber = request.OperativeNumber,
                    TnxLimit = request.TnxLimit,
                    DailyLimit = request.DailyLimit,
                    WeeklyLimit = request.WeeklyLimit,
                    MonthlyLimit = request.MonthlyLimit,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = (Status)request.Status,
                    TradeCardId = request.TradeCardId,
                    CreatedBy = "Test",
                    CreatedOn = DateTime.UtcNow,
                };

                this._context.Operatives.Add(operatives);
            }
            else
            {
                operatives.FirstName = request.FirstName;
                operatives.LastName = request.LastName;
                operatives.JobRole = request.JobRole;
                operatives.OperativeNumber = request.OperativeNumber;
                operatives.TnxLimit = request.TnxLimit;
                operatives.DailyLimit = request.DailyLimit;
                operatives.WeeklyLimit = request.WeeklyLimit;
                operatives.MonthlyLimit = request.MonthlyLimit;
                operatives.StartDate = request.StartDate;
                operatives.EndDate = request.EndDate;
                operatives.Status = (Status)request.Status;
                operatives.TradeCardId = request.TradeCardId;
                operatives.ModifiedBy = "Test";
                operatives.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<UpsertOperativeCommandResult>.Succeeded(
                new UpsertOperativeCommandResult(
                    operatives.Id,
                    operatives.FirstName,
                    operatives.LastName,
                    operatives.JobRole,
                    operatives.OperativeNumber,
                    operatives.TnxLimit,
                    operatives.DailyLimit,
                    operatives.WeeklyLimit,
                    operatives.MonthlyLimit,
                    operatives.StartDate,
                    operatives.EndDate,
                    (Client.Models.Status)operatives.Status,
                    operatives.TradeCardId));
        }
    }
}