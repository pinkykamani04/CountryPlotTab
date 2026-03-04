// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands;
using IBMG.SCS.Portal.Web.Infrastructure.Multitenancy;
using Microsoft.EntityFrameworkCore.Metadata;
using PearDrop.Authentication.Client.Constants;
using PearDrop.Authentication.Client.Domain.User.Commands;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers
{
    public sealed class UpdateCustomerUserCommandHandler
        : CommandHandler<UpdateCustomerUserCommand>
    {
        private readonly ICommander _commander;
        private readonly ITenantExecutionScope _tenantScope;

        public UpdateCustomerUserCommandHandler(
            IEnumerable<IValidator<UpdateCustomerUserCommand>> validators,
            ILogger<UpdateCustomerUserCommandHandler> logger,
            ICommander commander,
            ITenantExecutionScope tenantScope)
            : base(validators, logger)
        {
            this._commander = commander;
            this._tenantScope = tenantScope;
        }

        protected override async Task<CommandResult> HandleInternal(
            UpdateCustomerUserCommand request,
            CancellationToken cancellationToken)
        {
            using (this._tenantScope.Begin(request.TenantId))
            {
                var updateProfile = await this._commander.Send(new UpdateProfileCommand(request.UserId, request.FirstName, request.LastName, request.Email), cancellationToken);

                if (updateProfile.Status != BluQube.Constants.CommandResultStatus.Succeeded)
                {
                    return CommandResult.Failed(new BluQubeErrorData(updateProfile.ErrorData?.Message ?? "Profile update failed"));
                }

                if (request.Status == UserPrincipalNameStatus.Disabled)
                {
                    var disable = await this._commander.Send(new DisableUserPrincipalNameCommand(request.UserId, request.UserPrincipalId), cancellationToken);

                    if (disable.Status == BluQube.Constants.CommandResultStatus.Succeeded)
                    {
                        var disableAccount = await this._commander.Send(new DisableAccountCommand(request.UserId));

                        if (disableAccount.Status != BluQube.Constants.CommandResultStatus.Succeeded)
                        {
                            return CommandResult.Failed(new BluQubeErrorData(disable.ErrorData?.Message ?? "User disable failed"));
                        }
                    }
                    else
                    {
                        return CommandResult.Failed(new BluQubeErrorData(disable.ErrorData?.Message ?? "User disable failed"));
                    }
                }

                if (request.Status == UserPrincipalNameStatus.Verified)
                {
                    var unable = await this._commander.Send(new EnableAccountCommand(request.UserId));

                    if (unable.Status == BluQube.Constants.CommandResultStatus.Succeeded)
                    {
                        var test = await this._commander.Send(new VerifyUserPrincipleNameCommand(request.UserId, request.UserPrincipalId, Guid.NewGuid().ToString(), false));

                        if (test.Status != BluQube.Constants.CommandResultStatus.Succeeded)
                        {
                            return CommandResult.Failed(new BluQubeErrorData(test.ErrorData?.Message ?? "User Enable failed"));
                        }
                    }
                }

                return CommandResult.Succeeded();
            }
        }
    }
}