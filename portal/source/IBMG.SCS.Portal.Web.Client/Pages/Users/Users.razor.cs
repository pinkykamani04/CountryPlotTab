using BluQube.Constants;
using IBMG.SCS.Portal.Web.Client.Data;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.AspNetCore.Components.Web;
using PearDrop.Authentication.Client.Domain.User.Commands;
using Radzen;
using Radzen.Blazor;

namespace IBMG.SCS.Portal.Web.Client.Pages.Users
{
    public partial class Users
    {
        private string? search;
        private IReadOnlyList<QueryableUser> users = Array.Empty<QueryableUser>();
        private RadzenDataGrid<QueryableUser>? grid;
        private bool isLoading;
        private CancellationTokenSource? debounceCts;
        private bool showAdd;
        private bool saving;
        private CreateUserModel newUser = new();
        private Guid currentTenantId = Guid.Empty;

        //public PageLoadStatus PageLoadStatus { get; set; } = PageLoadStatus.NotStarted;

        protected override async Task OnInitializedAsync()
        {
            var authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            var tenantClaim = authState.User.FindFirst("TenantId");
            if (tenantClaim?.Value != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
            {
                this.currentTenantId = tenantId;
            }

            await this.Reload();
            //this.PageLoadStatus = PageLoadStatus.Loaded;
            await base.OnInitializedAsync();

        }

        private async Task OnSearchInput(object value)
        {
            this.search = value?.ToString();
            await this.DebouncedReload();
        }

        private async Task Reload()
        {
            isLoading = true;
            StateHasChanged();

            var result = await Querier.Send(
                new GetUsersQuery(search, false, currentTenantId),
                default);

            users = result.Status == QueryResultStatus.Succeeded
                ? result.Data.Users.ToList()
                : Array.Empty<QueryableUser>();

            isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task DebouncedReload(int delayMs = 300)
        {
            this.debounceCts?.Cancel();
            var cts = new CancellationTokenSource();
            this.debounceCts = cts;
            try
            {
                await Task.Delay(delayMs, cts.Token);
                await Reload();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        private void OnAddUser()
        {
            this.newUser = new();
            this.showAdd = true;
        }

        private void CloseAdd()
        {
            if (saving) return;
            this.showAdd = false;
        }

        private async Task SaveNewUser()
        {
            try
            {
                this.saving = true;
                var cmd = new CreateUserCommand(

                    UserId: Guid.NewGuid(),
                    ContactEmailAddress: newUser.Email!,
                    FirstName: newUser.FirstName!,
                    LastName: newUser.LastName!,
                    UserPrincipalNameValue: newUser.Email!,
                    MetaItems: null
                );

                var result = await this.Commander.Send(cmd, default);
                if (result.Status == CommandResultStatus.Succeeded)
                {
                    this.Notifier.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "User created", Duration = 3000 });
                    this.showAdd = false;
                    await Reload();
                }
                else
                {
                    this.Notifier.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Create failed", Detail = result.ErrorData.Message ?? "Unknown error", Duration = 5000 });
                }
            }
            catch (Exception ex)
            {
                this.Notifier.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Create failed", Detail = ex.Message, Duration = 6000 });
            }
            finally
            {
                this.saving = false;
                this.StateHasChanged();
            }
        }

        private sealed class CreateUserModel
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
        }
    }
}
