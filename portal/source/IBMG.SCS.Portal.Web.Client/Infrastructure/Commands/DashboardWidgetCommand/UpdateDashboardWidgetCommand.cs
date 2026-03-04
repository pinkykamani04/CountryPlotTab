// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand
{
    [BluQubeCommand(Path = "commands/dashboardwidget/update")]
    public record UpdateDashboardWidgetCommand(Guid DashboardId, int RowOrder, string RowLayoutType) : ICommand<UpdateDashboardWidgetCommandResult>;
}