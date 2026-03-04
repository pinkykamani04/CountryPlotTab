// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpdateJobRoleCommandResult
{
    public record UpdateJobRoleCommandResult(Guid Id, string Name, Guid? UserId) : ICommandResult;

}