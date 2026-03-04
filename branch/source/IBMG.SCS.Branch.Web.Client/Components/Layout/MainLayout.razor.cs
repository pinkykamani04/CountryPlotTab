// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;

namespace IBMG.SCS.Branch.Web.Client.Components.Layout
{
    public partial class MainLayout
    {
        private bool _isAuthenticating = true;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        private bool UserHasCustomersAssigned { get; set; }

        bool sidebarExpanded = true;

        protected override async Task OnInitializedAsync()
        {

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JSRuntime.InvokeVoidAsync("JsFunctions.detectTheme");
                await this.JSRuntime.InvokeVoidAsync("JsFunctions.registerResize", DotNetObjectReference.Create(this));

                var width = await this.JSRuntime.InvokeAsync<int>("JsFunctions.getWindowWidth");
                this.sidebarExpanded = width > 1024;
                this.StateHasChanged();
            }
        }

        [JSInvokable]
        public void OnWindowResize(int width)
        {
            if (width > 768 && !this.sidebarExpanded)
            {
                this.sidebarExpanded = true;
                this.StateHasChanged();
            }
            else if (width <= 768 && this.sidebarExpanded)
            {
                this.sidebarExpanded = false;
                this.StateHasChanged();
            }
        }

        private void OnMenuItemClick(MenuItemEventArgs args)
        {
            this.AutoCollapseOnMobile();
        }

        private void AutoCollapseOnMobile() => _ = this.CollapseSidebarIfMobile();

        private async Task CollapseSidebarIfMobile()
        {
            try
            {
                var width = await this.JSRuntime.InvokeAsync<int>("JsFunctions.getWindowWidth");
                if (width <= 1024)
                {
                    this.sidebarExpanded = false;
                    this.StateHasChanged();
                }
            }
            catch
            {
                this.sidebarExpanded = false;
                this.StateHasChanged();
            }
        }
    }
}