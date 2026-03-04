// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand
{
    [BluQubeCommand(Path = "commands/dashboardwidget/delete")]
    public record DeleteDashboardWidgetCommand(Guid Id) : ICommand;
}