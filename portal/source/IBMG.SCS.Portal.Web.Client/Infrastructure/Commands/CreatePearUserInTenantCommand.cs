// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using PearDrop.Authentication.Client.Domain.User.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands
{
    [BluQubeCommand(Path = "api/commands/auth/cross-tenant/pear-create-user")]
    public sealed record CreatePearUserInTenantCommand(
    Guid TenantId,
    Guid UserId,
    string ContactEmailAddress,
    string FirstName,
    string LastName,
    string UserPrincipalNameValue,
    IReadOnlyList<MetaItem>? MetaItems = null) : ICommand;
}