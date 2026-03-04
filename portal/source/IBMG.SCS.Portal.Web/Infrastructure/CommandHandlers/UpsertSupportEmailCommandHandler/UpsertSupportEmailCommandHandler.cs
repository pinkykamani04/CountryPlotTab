// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertSupportEmailCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.UpsertSupportEmailCommandHandler
{
    public class UpsertSupportEmailCommandHandler : CommandHandler<UpsertSupportEmailCommand, UpsertSupportEmailCommandResult>
    {
        private readonly PortalDBContext _context;

        public UpsertSupportEmailCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<UpsertSupportEmailCommand>> validators,
            ILogger<UpsertSupportEmailCommandHandler> logger)
            : base(validators, logger)
        {
            this._context = context;
        }

        protected override async Task<CommandResult<UpsertSupportEmailCommandResult>> HandleInternal(UpsertSupportEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var supportEmail = await this._context.Emails.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (supportEmail == null)
                {
                    supportEmail = new Emails()
                    {
                        Id = request.Id,
                        Email = request.Email,
                        CreatedBy = "Test",
                        CreatedOn = DateTime.UtcNow,
                    };

                    await this._context.Emails.AddAsync(supportEmail);
                }
                else
                {
                    supportEmail.Email = request.Email;
                    supportEmail.ModifiedOn = DateTime.UtcNow;
                    supportEmail.ModifiedBy = "Test";
                }

                await this._context.SaveChangesAsync(cancellationToken);

                return CommandResult<UpsertSupportEmailCommandResult>.Succeeded(new UpsertSupportEmailCommandResult(supportEmail.Id, supportEmail.Email));
            }
            catch (Exception ex)
            {
                return CommandResult<UpsertSupportEmailCommandResult>.Failed(new BluQubeErrorData($"Failed to save Support Email. {ex.Message}"));
            }
        }
    }
}