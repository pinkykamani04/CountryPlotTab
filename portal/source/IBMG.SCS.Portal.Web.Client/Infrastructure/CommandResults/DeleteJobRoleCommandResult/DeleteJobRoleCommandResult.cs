// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DeleteJobRoleCommandResult
{
    public record DeleteJobRoleCommandResult(Guid Id, Guid UserId) : ICommandResult;
}