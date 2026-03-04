using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand
{
    [BluQubeCommand(Path = "commands/dashboardwidget/add")]
    public record AddDashboardWidgetCommand(Guid Id, Guid DashboardId, int RowOrder, string RowLayoutType, int Position, Guid WidgetId, bool? IsRowDeleted) : ICommand<AddDashboardWidgetCommandResult>;
}