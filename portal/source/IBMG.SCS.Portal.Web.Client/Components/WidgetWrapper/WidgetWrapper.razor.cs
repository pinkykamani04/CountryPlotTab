// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace IBMG.SCS.Portal.Web.Client.Components.WidgetWrapper
{
    public partial class WidgetWrapper
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnAdd { get; set; }

        private Task OnAddClick(MouseEventArgs e)
            => OnAdd.HasDelegate ? OnAdd.InvokeAsync() : Task.CompletedTask;
    }
}