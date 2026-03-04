// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class DashboardWidgetClientDto
    {
        public Guid DashboardWidgetId { get; set; }

        public string WidgetName { get; set; } = default!;

        public int RowOrder { get; set; }

        public int Position { get; set; }

        public string RowLayoutType { get; set; } = "4";
    }
}