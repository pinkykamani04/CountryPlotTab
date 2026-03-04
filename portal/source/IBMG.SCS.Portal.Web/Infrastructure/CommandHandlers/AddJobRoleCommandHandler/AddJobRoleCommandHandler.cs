using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddJobRoleCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddJobRoleCommand;
using Microsoft.EntityFrameworkCore;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.AddJobRoleCommandHandler
{
    public class AddJobRoleCommandHandler : CommandHandler<AddJobRoleCommand, AddJobRoleCommandResult>
    {
        private readonly ICurrentAuthenticatedUserProvider _userProvider;
        private readonly PortalDBContext _context;
        Guid userId;

        public AddJobRoleCommandHandler(
            ICurrentAuthenticatedUserProvider userProvider,
            PortalDBContext context,
            IEnumerable<IValidator<AddJobRoleCommand>> validators,
            ILogger<AddJobRoleCommandHandler> logger)
            : base(validators, logger)
        {
            this._userProvider = userProvider;
            this._context = context;
        }

        protected override async Task<CommandResult<AddJobRoleCommandResult>> HandleInternal(
            AddJobRoleCommand request,
            CancellationToken cancellationToken)
        {
            var tradeCard = await this._context.JobRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            var current = this._userProvider.CurrentAuthenticatedUser;

            if (current.HasValue)
            {
                this.userId = current.Value.UserId;
            }

            if (tradeCard == null)
            {
                tradeCard = new JobRoles
                {
                    Id = request.Id,
                    Name = request.Name,
                    UserId = userId,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "Test",
                    IsRowDeleted = false,
                };

                this._context.JobRoles.Add(tradeCard);
            }
            else
            {
                tradeCard.Name = request.Name;
                tradeCard.ModifiedBy = "Test";
                tradeCard.ModifiedOn = DateTime.UtcNow;
            }

            await this._context.SaveChangesAsync(cancellationToken);

            return CommandResult<AddJobRoleCommandResult>.Succeeded(
                new AddJobRoleCommandResult(
                    tradeCard.Id,
                    tradeCard.Name,
                    tradeCard.UserId));
        }
    }
}