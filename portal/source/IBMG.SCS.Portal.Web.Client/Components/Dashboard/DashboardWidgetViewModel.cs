using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Components.Dashboard
{
    public class DashboardWidgetViewModel
    {
        public Guid DashboardWidgetId { get; set; }

        public Guid DashboardId { get; set; }

        public Guid WidgetId { get; set; }

        public int RowOrder { get; set; }

        public int Position { get; set; }

        public string RowLayoutType { get; set; } = "4";

        public RenderFragment? Content { get; set; }

        public string? GroupByColumn { get; set; } = "Branch";

        public string Title { get; set; } = "";

    }
}