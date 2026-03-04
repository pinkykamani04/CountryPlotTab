// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddTradeCardCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddTradeCardCommand;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.AddTradeCardCommandHandler
{
    public class AddTradeCardCommandHandler
    : CommandHandler<AddTradeCardCommand, AddTradeCardCommandResult>
    {
        private readonly PortalDBContext _context;

        public AddTradeCardCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<AddTradeCardCommand>> validators,
            ILogger<AddTradeCardCommandHandler> logger)
            : base(validators, logger)
        {
            this._context = context;

        }

        protected override async Task<CommandResult<AddTradeCardCommandResult>> HandleInternal(
            AddTradeCardCommand request,
            CancellationToken cancellationToken)
        {
            var tradeCard = await this._context.TradeCards.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (tradeCard == null)
            {
                tradeCard = new TradeCards
                {
                    Id = request.Id,
                    TradeCardNumber = request.TradeCardNumber,
                    AssigneeId = request.AssigneeId,
                    Status = request.Status,
                    CreatedBy = "Test",
                    CreatedOn = DateTime.UtcNow,
                    IsRowDeleted = false,
                };

                this._context.TradeCards.Add(tradeCard);
            }
            else
            {
                tradeCard.TradeCardNumber = request.TradeCardNumber;
                tradeCard.AssigneeId = request.AssigneeId;
                tradeCard.Status = request.Status;
                tradeCard.ModifiedBy = "Test";
                tradeCard.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<AddTradeCardCommandResult>.Succeeded(
                new AddTradeCardCommandResult(
                    tradeCard.Id,
                    tradeCard.TradeCardNumber,
                    tradeCard.AssigneeId,
                    tradeCard.Status));
        }
    }
}