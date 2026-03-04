// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand
{
    [BluQubeCommand(Path = "commands/dashboard/delete")]
    public record DeleteDashboardCommand(Guid Id) : ICommand;
}