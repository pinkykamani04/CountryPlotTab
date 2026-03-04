// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertOperativeCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertValidationCommandResult;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertValidationCommand
{
    [BluQubeCommand(Path = "commands/validation/add")]
    public record UpsertValidationCommand(Guid Id, int UserId, int ValidationType, bool IsEnabled, bool IsMandatory, DateTime CreatedOn, DateTime? ModifiedOn, Guid? ModifiedBy )
     : ICommand<UpsertValidationCommandResult>;
}