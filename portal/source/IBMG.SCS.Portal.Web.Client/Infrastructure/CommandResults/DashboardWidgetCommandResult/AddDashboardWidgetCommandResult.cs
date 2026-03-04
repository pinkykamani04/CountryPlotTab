// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DashboardWidgetCommandResult;

public record AddDashboardWidgetCommandResult(Guid Id, Guid DashboardId, int RowOrder, string RowLayoutType, int Position, Guid WidgetId) : ICommandResult;
