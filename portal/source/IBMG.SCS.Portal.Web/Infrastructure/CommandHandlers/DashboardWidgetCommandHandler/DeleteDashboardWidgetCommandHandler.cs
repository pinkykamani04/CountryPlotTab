using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardWidgetCommandHandler
{
    public class DeleteDashboardWidgetCommandHandler(IDashboardService dashboardService, IEnumerable<IValidator<DeleteDashboardWidgetCommand>> validators, ILogger<DeleteDashboardWidgetCommandHandler> logger)
    : CommandHandler<DeleteDashboardWidgetCommand>(validators, logger)
    {
        protected override async Task<CommandResult> HandleInternal(DeleteDashboardWidgetCommand request, CancellationToken cancellationToken)
        {
            bool result = await dashboardService.RemoveWidgetAsync(request.Id);

            return result
                ? CommandResult.Succeeded()
                : CommandResult.Failed(new BluQubeErrorData("Failed to delete Dashboard."));
        }
    }
}