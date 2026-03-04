// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.SideDialog
{
    public partial class SideDialog : ComponentBase
    {
        [Inject]
        private ICommander Commander { get; set; } = default!;

        [Inject]
        private NotificationService NotificationService { get; set; } = default!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        [Parameter]
        public string EmailAddress { get; set; }

        private bool IsBusy = false;

        private ReportIssueDto model = new();

        private readonly List<string> issueTypes = new()
        {
            "Portal Issue",
            "Account Issue",
            "Other",
        };

        private async Task OnSubmit()
        {
            this.IsBusy = true;

            var command = await this.Commander.Send(new ReportIssueCommand(this.EmailAddress, model.IssueType, model.IssueDetails));

            if (command.Status == CommandResultStatus.Succeeded)
            {
                this.NotificationService.Notify(NotificationSeverity.Success, "Your issue has been reported successfully.");
                await this.DialogService.CloseSideAsync();
            }
            else
            {
                this.NotificationService.Notify(NotificationSeverity.Error, "Failed to send the issue. Please try again later.");
            }

            this.IsBusy = false;
        }
    }
}