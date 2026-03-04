using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardWidgetCommandHandler
{
    public class UpdateDashboardWidgetCommandHandler
     : CommandHandler<UpdateDashboardWidgetCommand, UpdateDashboardWidgetCommandResult>
    {
        private readonly IDashboardService _dashboardService;

        public UpdateDashboardWidgetCommandHandler(
            IDashboardService dashboardService,
            IEnumerable<IValidator<UpdateDashboardWidgetCommand>> validators,
            ILogger<UpdateDashboardWidgetCommandHandler> logger)
            : base(validators, logger)
        {
            _dashboardService = dashboardService;
        }

        protected override async Task<CommandResult<UpdateDashboardWidgetCommandResult>> HandleInternal(
            UpdateDashboardWidgetCommand request,
            CancellationToken cancellationToken)
        {
            var updatedWidgets = await _dashboardService.UpdateDashboardWidgetLayoutAsync(
                request.DashboardId,
                request.RowOrder,
                request.RowLayoutType
            );

            if (!updatedWidgets.Any())
            {
                return CommandResult<UpdateDashboardWidgetCommandResult>.Failed(new BluQubeErrorData("No widgets found for the given row."));
            }

            var result = new UpdateDashboardWidgetCommandResult(
                request.DashboardId,
                request.RowOrder,
                request.RowLayoutType
            );

            return CommandResult<UpdateDashboardWidgetCommandResult>.Succeeded(result);
        }
    }
}