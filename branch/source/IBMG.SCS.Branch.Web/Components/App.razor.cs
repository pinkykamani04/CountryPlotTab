// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace IBMG.SCS.Branch.Web.Components;

public partial class App
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private IComponentRenderMode? PageRenderMode =>
        this.HttpContext.AcceptsInteractiveRouting() ? new InteractiveWebAssemblyRenderMode(prerender: false) : null;
}