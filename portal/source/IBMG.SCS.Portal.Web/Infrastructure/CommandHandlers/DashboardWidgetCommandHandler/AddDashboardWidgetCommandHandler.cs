using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardWidgetCommandHandler;

   public class AddDashboardWidgetCommandHandler(IDashboardService dashboardService, IEnumerable<IValidator<AddDashboardWidgetCommand>> validators, ILogger<AddDashboardWidgetCommandHandler> logger)
: CommandHandler<AddDashboardWidgetCommand, AddDashboardWidgetCommandResult>(validators, logger)
{
    protected override async Task<CommandResult<AddDashboardWidgetCommandResult>> HandleInternal(
     AddDashboardWidgetCommand request,
     CancellationToken cancellationToken)
    {
        var widget = await dashboardService.AddWidgetToDashboardAsync(
            request.Id,
            request.DashboardId,
            request.RowOrder,
            request.RowLayoutType,
            request.Position,
            request.WidgetId
        );

        return CommandResult<AddDashboardWidgetCommandResult>.Succeeded(
            new AddDashboardWidgetCommandResult(
                widget.Id,
                widget.DashboardId,
                widget.RowOrder,
                widget.RowLayoutType,
                widget.Position,
                widget.WidgetId
            )
        );
    }
}