// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertSupportEmailCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand
{
    [BluQubeCommand(Path = "commands/support-email/upsert")]
    public record UpsertSupportEmailCommand(Guid Id, string Email) : ICommand<UpsertSupportEmailCommandResult>;
}