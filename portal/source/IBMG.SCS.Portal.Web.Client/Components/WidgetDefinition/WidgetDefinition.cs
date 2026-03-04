// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Components.WidgetDefinition
{
    public class WidgetDefinition
    {
        public string WidgetKey { get; set; } = default!;

        public RenderFragment? Content { get; set; }
    }
}