// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardCommandHandler
{
    public class DeleteDashboardCommandHandler(IDashboardService dashboardService, IEnumerable<IValidator<DeleteDashboardCommand>> validators, ILogger<DeleteDashboardCommandHandler> logger)
    : CommandHandler<DeleteDashboardCommand>(validators, logger)
    {
        protected override Task<CommandResult> HandleInternal(DeleteDashboardCommand request, CancellationToken cancellationToken)
        {
            var result = dashboardService.DeleteDashboard(request.Id);
            return Task.FromResult(result ? CommandResult.Succeeded() : CommandResult.Failed(new BluQubeErrorData("Failed to delete Dashboard.")));
        }
    }
}