// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using PearDrop.Authentication.Client.Constants;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands
{
    [BluQubeCommand(Path = "api/commands/update-customer-user")]
    public sealed record UpdateCustomerUserCommand(Guid TenantId, Guid UserId, Guid UserPrincipalId, string FirstName,
         string LastName, string Email, UserPrincipalNameStatus Status) : ICommand;
}