// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertValidationCommandResult
{
    public record UpsertValidationCommandResult(Guid Id, int UserId, int ValidationType, bool IsEnabled, bool IsMandatory, DateTime CreatedOn, DateTime? ModifiedOn, Guid? ModifiedBy) : ICommandResult;
}
