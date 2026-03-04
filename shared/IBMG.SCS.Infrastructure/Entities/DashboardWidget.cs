// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class DashboardWidget
    {
        public Guid Id { get; set; }

        public Guid DashboardId { get; set; }

        public int RowOrder { get; set; }

        public string RowLayoutType { get; set; } 

        public int Position { get; set; }

        public Guid WidgetId { get; set; }

        public bool? IsRowDeleted { get; set; }

        public DashboardItem? Dashboard { get; set; }

        public Widget? Widget { get; set; }
    }
}