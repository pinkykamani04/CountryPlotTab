// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddJobRoleCommandResult
{
    public record AddJobRoleCommandResult(Guid Id, string Name, Guid? UserId) : ICommandResult;
}