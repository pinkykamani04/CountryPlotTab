// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands;
using IBMG.SCS.Portal.Web.Infrastructure.Multitenancy;
using PearDrop.Authentication.Client.Domain.User.Commands;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers
{
    public sealed class CreateUserInTenantCommandHandler
    : CommandHandler<CreatePearUserInTenantCommand>
    {
        private readonly ICommander _commander;
        private readonly ITenantExecutionScope _tenantScope;

        public CreateUserInTenantCommandHandler(
            IEnumerable<IValidator<CreatePearUserInTenantCommand>> validators,
            ILogger<CreateUserInTenantCommandHandler> logger,
            ICommander commander,
            ITenantExecutionScope tenantScope)
            : base(validators, logger)
        {
            _commander = commander;
            _tenantScope = tenantScope;
        }

        protected override async Task<CommandResult> HandleInternal(
            CreatePearUserInTenantCommand request,
            CancellationToken cancellationToken)
        {
            using (_tenantScope.Begin(request.TenantId))
            {
                var targetCommand = new CreateUserCommand(
                    request.UserId,
                    request.ContactEmailAddress,
                    request.FirstName,
                    request.LastName,
                    request.UserPrincipalNameValue,
                    request.MetaItems
                );

                return await _commander.Send(targetCommand, cancellationToken);
            }
        }
    }
}
