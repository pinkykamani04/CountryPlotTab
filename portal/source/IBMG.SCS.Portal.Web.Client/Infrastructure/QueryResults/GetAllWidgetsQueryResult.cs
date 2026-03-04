// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllWidgetsQueryResult(IReadOnlyList<GetAllWidgetsQueryResult.WidgetItem> widgetItems) : IQueryResult
    {
        public IReadOnlyList<WidgetItem> WidgetItems { get; } = widgetItems;

        public record WidgetItem(
        Guid Id,
        string Name,
        bool IsMandatory,
        string Icon
    );
    }
}