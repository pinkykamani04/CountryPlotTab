// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertSupportEmailCommandResult
{
    public record UpsertSupportEmailCommandResult(Guid Id, string Email) : ICommandResult;
}