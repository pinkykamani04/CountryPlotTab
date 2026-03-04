// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.DashboardCommandHandler;
public class AddDashboardCommandHandler(IDashboardService dashboardService, IEnumerable<IValidator<AddDashboardCommand>> validators, ILogger<AddDashboardCommandHandler> logger)
: CommandHandler<AddDashboardCommand>(validators, logger)
{
    protected override Task<CommandResult> HandleInternal(AddDashboardCommand request, CancellationToken cancellationToken)
    {
        dashboardService.AddDashboard(request.Id, request.UserId, request.CustomerId, request.TemplateType);

        return Task.FromResult(CommandResult.Succeeded());
    }

}