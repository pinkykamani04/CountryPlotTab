// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddJobRoleCommand;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Components.JobRole
{
    public partial class JobRoleSlideOut : ComponentBase
    {
        [Parameter]
        public JobRoleDto JobRole { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public EventCallback<JobRoleDto> OnSubmit { get; set; }

        private async Task OnSave()
        {
            var id = this.JobRole.Id == Guid.Empty ? Guid.NewGuid() : this.JobRole.Id;

            var result = await this.Commander.Send(
                new AddJobRoleCommand(id, this.JobRole.Name, Guid.NewGuid())
            );

            if (result.Status != BluQube.Constants.CommandResultStatus.Succeeded)
            {
                return;
            }

            await this.OnSubmit.InvokeAsync(this.JobRole);

            this.DialogService.Close(this.JobRole);
        }
    }
}