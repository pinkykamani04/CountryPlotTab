// Copyright (c) IBMG. All rights reserved.

using BluQube.Constants;
using IBMG.SCS.Portal.Web.Client.Components.CustomerUser;
using IBMG.SCS.Portal.Web.Client.Data;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.AspNetCore.Components;
using PearDrop.Authentication.Client.Constants;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Pages.Customer
{
    public partial class TenantDetails : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Parameter]
        public Guid TenantId { get; set; }

        private string searchText { get; set; } = string.Empty;

        private QueryableTenant? tenant;

        private GetUsersQueryResult? users;

        private IEnumerable<QueryableUser> FilteredUsers => this.users?.Users == null ? Enumerable.Empty<QueryableUser>()
                            : string.IsNullOrWhiteSpace(this.searchText)
                                ? this.users.Users : this.users.Users.Where(u =>
                                    (!string.IsNullOrWhiteSpace(u.Firstname) &&
                                     u.Firstname.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                                    || (!string.IsNullOrWhiteSpace(u.Lastname) &&
                                     u.Lastname.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                                    || (!string.IsNullOrWhiteSpace(u.UserPrincipalName) &&
                                     u.UserPrincipalName.Contains(this.searchText, StringComparison.OrdinalIgnoreCase)));

        protected override async Task OnParametersSetAsync()
        {
            var result = await this.Querier.Send(new GetTenantByIdQuery(this.TenantId));

            this.tenant = result.Status == QueryResultStatus.Succeeded ? result.Data : null;

            await this.LoadUsers();
        }

        private async Task LoadUsers()
        {
            this.StateHasChanged();

            var result = await this.Querier.Send(new GetUsersQuery(null, false, this.TenantId));

            this.users = result.Status == QueryResultStatus.Succeeded ? result.Data : null;
        }

        private async Task OpenEditUser(QueryableUser user)
        {
            var authPrin = await this.Querier.Send(new GetCustomerUserPrincipalIdQuery(user.Id, this.TenantId));

            var model = new UserEditDto
            {
                Id = user.Id,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.UserPrincipalName,
                Status = user.Status,
            };

            this.DialogService.Open<CustomerEditSlideout>(
                string.Empty,
                new Dictionary<string, object>
                {
            { "User", model },
            { "IsEdit", true },
            { "TenantId", this.TenantId },
            { "UserPrincipalId", authPrin.Data.UserPrincipalNameId },
            { "OnSubmit", EventCallback.Factory.Create<UserEditDto>(this, this.OnUserSaved) },
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

        private async Task OnUserSaved(UserEditDto model)
        {
            await this.LoadUsers();
            this.StateHasChanged();
        }

        private void GoBack() => this.Nav.NavigateTo("/customers");

        private void CreateUser() => this.Nav.NavigateTo($"/customers/{this.TenantId}/users/create");
    }
}