// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand
{
    [BluQubeCommand(Path = "commands/dashboard/add")]
    public record AddDashboardCommand(Guid Id, Guid? UserId, Guid? CustomerId, int TemplateType) : ICommand;
}