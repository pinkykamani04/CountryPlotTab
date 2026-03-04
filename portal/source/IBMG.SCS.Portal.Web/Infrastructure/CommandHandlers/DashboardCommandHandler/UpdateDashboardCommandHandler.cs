// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardCommandHandler
{
    public class UpdateDashboardCommandHandler(IDashboardService dashboardService, IEnumerable<IValidator<UpdateDashboardCommand>> validators, ILogger<UpdateDashboardCommandHandler> logger)
: CommandHandler<UpdateDashboardCommand>(validators, logger)
    {
        protected override Task<CommandResult> HandleInternal(UpdateDashboardCommand request, CancellationToken cancellationToken)
        {
            var todo = dashboardService.UpdateDashboard(request.Id, request.UserId, request.CustomerId, request.TemplateType);

            return Task.FromResult(CommandResult.Succeeded());
        }
    }
}