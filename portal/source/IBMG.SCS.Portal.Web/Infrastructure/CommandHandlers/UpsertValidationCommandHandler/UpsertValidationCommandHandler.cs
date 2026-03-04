// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddJobRoleCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertValidationCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddJobRoleCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertValidationCommand;
using Microsoft.EntityFrameworkCore;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.UpsertValidationCommandHandler
{
    public class UpsertValidationCommandHandler : CommandHandler<UpsertValidationCommand, UpsertValidationCommandResult>
    {
        private readonly ICurrentAuthenticatedUserProvider _userProvider;
        private readonly PortalDBContext _context;
        Guid userId;

        public UpsertValidationCommandHandler(
            ICurrentAuthenticatedUserProvider userProvider,
            PortalDBContext context,
            IEnumerable<IValidator<UpsertValidationCommand>> validators,
            ILogger<UpsertValidationCommandHandler> logger)
            : base(validators, logger)
        {
            this._userProvider = userProvider;
            this._context = context;
        }

        protected override async Task<CommandResult<UpsertValidationCommandResult>> HandleInternal(
            UpsertValidationCommand request,
            CancellationToken cancellationToken)
        {
            var current = this._userProvider.CurrentAuthenticatedUser;
            var validation = await this._context.Validations.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ValidationType == request.ValidationType, cancellationToken);

            if (validation == null)
            {
                validation = new Validation
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    ValidationType = request.ValidationType,
                    IsEnabled = request.IsEnabled,
                    IsMandatory = request.IsMandatory,
                    CreatedOn = request.CreatedOn,
                    ModifiedOn = null,
                    ModifiedBy = null,
                };

                this._context.Validations.Add(validation);
            }
            else
            {
                validation.ValidationType = request.ValidationType;
                validation.IsEnabled = request.IsEnabled;
                validation.IsMandatory = request.IsMandatory;
                validation.ModifiedOn = DateTime.UtcNow;
                validation.ModifiedBy = this.userId;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<UpsertValidationCommandResult>.Succeeded(
                new UpsertValidationCommandResult(
                    validation.Id,
                    validation.UserId,
                    validation.ValidationType,
                    validation.IsEnabled,
                    validation.IsMandatory,
                    validation.CreatedOn,
                    validation.ModifiedOn,
                    validation.ModifiedBy));
        }
    }
}
