// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using IBMG.SCS.Portal.Web.Client.Components.JobRole;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DeleteJobRoleCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Pages.JobRoles
{
    public partial class JobRoles : ComponentBase
    {
        private string searchText = string.Empty;

        private List<JobRoleDto> Roles { get; set; } = new();

        [Inject]
        public ICommander Commander { get; set; } = default!;

        private List<JobRoleDto> FilteredRoles =>
            string.IsNullOrWhiteSpace(searchText)
            ? this.Roles
            : this.Roles.Where(x => x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

        protected override async Task OnInitializedAsync()
        {
            await this.LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            var result = await this.Querier.Send(new GetAllJobRoleQuery());

            if (result?.Status == QueryResultStatus.Succeeded && result.Data != null)
            {
                this.Roles = result.Data.LimitDtos.ToList();
            }
        }

        private void OpenAdd()
        {
            var model = new JobRoleDto();

            this.DialogService.Open<JobRoleSlideOut>(
                string.Empty,
                new Dictionary<string, object>()
                {
                { "JobRole", model },
                { "IsEdit", false },
                { "OnSubmit", EventCallback.Factory.Create<JobRoleDto>(this, this.OnRoleSaved) },
                },
                new DialogOptions
                {
                    Width = "350px",
                    Height = "100%",
                    CssClass = "custom-slide-out",
                    Style = "right:0;",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });
        }

        private void OpenEdit(JobRoleDto model)
        {
            var editModel = new JobRoleDto
            {
                Id = model.Id,
                Name = model.Name,
                UserId = model.UserId,
            };

            this.DialogService.Open<JobRoleSlideOut>(
                string.Empty,
                new Dictionary<string, object>()
                {
                { "JobRole", editModel },
                { "IsEdit", true },
                { "OnSubmit", EventCallback.Factory.Create<JobRoleDto>(this, this.OnRoleSaved) },
                },
                new DialogOptions
                {
                    Width = "350px",
                    Height = "100%",
                    CssClass = "custom-slide-out",
                    Style = "right:0;",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });
        }

        private async Task OnRoleSaved(JobRoleDto dto)
        {
            await this.LoadRolesAsync();
            this.StateHasChanged();
        }

        private async Task DeleteRole(Guid roleId)
        {
            bool? confirmed = await this.DialogService.Confirm(
                "Are you sure you want to delete this role?",
                "Confirm Delete",
                new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                });

            if (confirmed == true)
            {
                var result = await this.Commander.Send(new DeleteJobRoleCommand(roleId, Guid.NewGuid()));

                if (result.Status == CommandResultStatus.Succeeded)
                {
                    await this.LoadRolesAsync();
                    this.StateHasChanged();
                }
            }
        }
    }
}