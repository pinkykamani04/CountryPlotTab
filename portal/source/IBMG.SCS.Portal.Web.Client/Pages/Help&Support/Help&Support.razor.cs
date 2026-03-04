// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Components.SideDialog;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Pages.Help_Support
{
    public partial class Help_Support : ComponentBase
    {
        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private DialogService DialogService { get; set; } = default!;

        public string EmailAddress = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await this.LoadHelpSupportEmailAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadHelpSupportEmailAsync()
        {
            var email = await this.Querier.Send(new GetAllSupportEmailQuery());

            if (email.Status == BluQube.Constants.QueryResultStatus.Succeeded && !string.IsNullOrEmpty(email.Data.EmailDtos.SupportEmail))
            {
                this.EmailAddress = email.Data.EmailDtos.SupportEmail;
            }
            else
            {
                // Used if Admin not set the support email
                this.EmailAddress = "Adam.Dexter@independentbm.com";
            }
        }

        private async Task OpenSideDialog()
        {
            await this.DialogService.OpenSideAsync<SideDialog>
            ("",
            new Dictionary<string, object>
            {
                { "EmailAddress", this.EmailAddress },
            },
            options: new SideDialogOptions
                {
                    CloseDialogOnOverlayClick = true,
                    Position = DialogPosition.Right,
                    ShowMask = true,
                    CssClass = "custom-slide-out",
                    ShowClose = true,
                    Height = "100%",
                });
        }
    }
}