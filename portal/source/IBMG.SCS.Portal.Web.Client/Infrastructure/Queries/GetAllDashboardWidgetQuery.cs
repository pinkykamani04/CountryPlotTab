// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;

[BluQubeQuery(Path = "queries/dashboardwidget/get-all")]
public record GetAllDashboardWidgetQuery : IQuery<GetAllDashboardWidgetQueryResult>;