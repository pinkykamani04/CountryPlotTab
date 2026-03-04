// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Pages.SupportEmail
{
    public partial class SupportEmail : ComponentBase
    {
        [Inject]
        private ICommander Commander { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private NotificationService NotificationService { get; set; } = default!;

        private EmailDto model = new();

        private bool isEditMode = false;

        protected override async Task OnInitializedAsync()
        {
            await this.LoadSupportEmailAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadSupportEmailAsync()
        {
            var emails = await this.Querier.Send(new GetAllSupportEmailQuery());

            if (emails.Status == QueryResultStatus.Succeeded && emails.Data.EmailDtos != null)
            {
                this.model = new EmailDto()
                {
                    Id = emails.Data.EmailDtos.Id,
                    SupportEmail = emails.Data.EmailDtos.SupportEmail,
                };

                this.isEditMode = false;
            }
        }

        private void EnableEdit()
        {
            isEditMode = true;
        }

        private async Task OnSave(EmailDto model)
        {
            var id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id;

            var command = await this.Commander.Send(new UpsertSupportEmailCommand(id, model.SupportEmail));

            if (command.Status == CommandResultStatus.Succeeded)
            {
                isEditMode = false;
                this.NotificationService.Notify(NotificationSeverity.Success, "Email Added Successfully.");
            }
            else
            {
                this.NotificationService.Notify(NotificationSeverity.Error, command.ErrorData.Message);
            }
        }
    }
}