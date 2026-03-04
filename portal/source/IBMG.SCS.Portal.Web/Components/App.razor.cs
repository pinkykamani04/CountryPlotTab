// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace IBMG.SCS.Portal.Web.Components;

public partial class App(AuthenticationStateProvider authStateProvider, NavigationManager nav)
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private IComponentRenderMode? PageRenderMode =>
        this.HttpContext.AcceptsInteractiveRouting() ? new InteractiveWebAssemblyRenderMode(prerender: false) : null;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            var relative = nav.ToBaseRelativePath(nav.Uri);
            var localPath = string.IsNullOrWhiteSpace(relative) ? "/" : (relative.StartsWith('/') ? relative : "/" + relative);
            var returnUrl = Uri.EscapeDataString(localPath);
            nav.NavigateTo($"/auth/sign-in?returnUrl={returnUrl}", forceLoad: true);
            return;
        }
    }
}