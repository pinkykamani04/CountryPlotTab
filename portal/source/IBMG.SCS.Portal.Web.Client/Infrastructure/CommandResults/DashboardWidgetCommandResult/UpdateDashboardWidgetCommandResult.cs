// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;

public record UpdateDashboardWidgetCommandResult(Guid DashboardId, int RowOrder, string RowLayoutType) : ICommandResult;
