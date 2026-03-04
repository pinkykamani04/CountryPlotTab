// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands;
using Microsoft.AspNetCore.Components;
using PearDrop.Authentication.Client.Constants;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.CustomerUser
{
    public partial class CustomerEditSlideout : ComponentBase
    {
        [Inject]
        private ICommander Commander { get; set; } = default!;

        [Parameter]
        public UserEditDto User { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public Guid TenantId { get; set; }

        [Parameter]
        public Guid UserPrincipalId { get; set; }

        [Parameter]
        public EventCallback<UserEditDto> OnSubmit { get; set; }

        [Inject]
        private DialogService DialogService { get; set; } = default!;

        [Inject]
        private NotificationService NotificationService { get; set; } = default!;

        private IReadOnlyList<StatusOption> StatusOptions => new[] {
                new StatusOption
                {
                    Text = "Verified",
                    Value = UserPrincipalNameStatus.Verified,
                },
                new StatusOption
                {
                    Text = "Disabled",
                    Value = UserPrincipalNameStatus.Disabled,
                },
            };

        private sealed class StatusOption
        {
            public string Text { get; set; } = string.Empty;

            public UserPrincipalNameStatus Value { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (this.User.Status is UserPrincipalNameStatus.Unverified or UserPrincipalNameStatus.NoVerificationNeeded)
            {
                this.User.Status = UserPrincipalNameStatus.Verified;
            }
        }

        private async Task SaveUser(UserEditDto model)
        {
            var test = await this.Commander.Send(new UpdateCustomerUserCommand(this.TenantId, model.Id, this.UserPrincipalId, model.FirstName,
                                                                                model.LastName, model.Email, model.Status));

            if (test.Status == BluQube.Constants.CommandResultStatus.Succeeded)
            {
                this.NotificationService.Notify(NotificationSeverity.Success, "Updated Successfully!");
            }
            else
            {
                this.NotificationService.Notify(NotificationSeverity.Error, test.ErrorData.Message.ToString());
            }

            await this.OnSubmit.InvokeAsync(model);
            this.DialogService.Close();
        }
    }
}