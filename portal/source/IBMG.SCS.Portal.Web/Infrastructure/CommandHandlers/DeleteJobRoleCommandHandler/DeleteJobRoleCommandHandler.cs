// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DeleteJobRoleCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DeleteJobRoleCommand;
using Microsoft.EntityFrameworkCore;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DeleteJobRoleCommandHandler
{
    public class DeleteJobRoleCommandHandler : CommandHandler<DeleteJobRoleCommand, DeleteJobRoleCommandResult>
    {
        private readonly ICurrentAuthenticatedUserProvider _userProvider;
        private readonly PortalDBContext _context;
        Guid UserId;

        public DeleteJobRoleCommandHandler(
            ICurrentAuthenticatedUserProvider userProvider,
            PortalDBContext context,
            IEnumerable<IValidator<DeleteJobRoleCommand>> validators,
            ILogger<DeleteJobRoleCommandHandler> logger)
            : base(validators, logger)
        {
            this._userProvider = userProvider;
            this._context = context;
        }

        protected override async Task<CommandResult<DeleteJobRoleCommandResult>> HandleInternal(
            DeleteJobRoleCommand request,
            CancellationToken cancellationToken)
        {
            var current = this._userProvider.CurrentAuthenticatedUser;

            if (current.HasValue)
            {
                this.UserId = current.Value.UserId;
            }

            var jobRole = await this._context.JobRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (jobRole != null)
            {
                jobRole.IsRowDeleted = true;
                jobRole.ModifiedBy = this.UserId.ToString();
                jobRole.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<DeleteJobRoleCommandResult>.Succeeded(
                new DeleteJobRoleCommandResult(
                   jobRole.Id,
                   this.UserId));
        }
    }
}