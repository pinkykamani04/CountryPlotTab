// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class Widget
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }

        public string Icon { get; set; } = string.Empty;

        public ICollection<DashboardWidget>? DashboardWidgets { get; set; }
    }
}