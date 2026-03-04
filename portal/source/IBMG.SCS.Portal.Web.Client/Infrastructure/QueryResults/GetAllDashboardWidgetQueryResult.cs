// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllDashboardWidgetQueryResult : IQueryResult
    {
        public List<WidgetItems> Items { get; set; } = new();

        public record WidgetItems(
            Guid Id,
            Guid DashboardId,
            int RowOrder,
            string RowLayoutType,
            int Position,
            Guid WidgetId,
            bool? IsRowDeleted
        );
    }
}