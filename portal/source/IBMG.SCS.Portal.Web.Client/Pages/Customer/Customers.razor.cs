// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.ApiClient;
using IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using PearDrop.Multitenancy.Client.Domain.Tenant.Commands;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Pages.Customer
{
    public partial class Customers : ComponentBase
    {
        [Inject]
        private IClient Client { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private ICommander Commander { get; set; } = default!;

        [Inject]
        private NotificationService NotificationService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private List<ScsCustomer> CustomerList { get; set; } = new();

        private string searchText { get; set; } = string.Empty;

        private GetTenantsQueryResult? tenants;

        private Dictionary<string, QueryableTenant> tenantByIdentifier = new();

        private bool isLoading;

        private IEnumerable<ScsCustomer> FilteredCustomers => string.IsNullOrWhiteSpace(this.searchText) ? this.CustomerList
                                    : this.CustomerList.Where(c => (!string.IsNullOrEmpty(c.Customer_Code) &&
                                         c.Customer_Code.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                                        || (!string.IsNullOrEmpty(c.Customer_Desc) &&
                                         c.Customer_Desc.Contains(this.searchText, StringComparison.OrdinalIgnoreCase)));

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            await LoadTenantsAsync();
            await LoadCustomerAsync();

            StateHasChanged();

            _ = Task.Run(async () =>
            {
                if (CustomerList.Any())
                {
                    await EnsureTenantsForCustomersAsync();
                }
            });

            isLoading = false;
            await base.OnInitializedAsync();
        }

        private async Task LoadCustomerAsync()
        {
            var customers = await this.Client.ListCustomerPortalCustomersAsync();

            if (customers.Success && customers.Data.Count != 0 && customers.Data.Any())
            {
                this.CustomerList = customers.Data.ToList();
            }
            else
            {
                this.NotificationService.Notify(NotificationSeverity.Error, customers.Message);
            }
        }

        private async Task LoadTenantsAsync()
        {
            var result = await this.Querier.Send(new GetTenantsQuery(this.searchText));
            this.tenants = result.Status == QueryResultStatus.Succeeded && result.Data != null ? result.Data : new GetTenantsQueryResult(Array.Empty<QueryableTenant>());
            tenantByIdentifier = this.tenants.Tenants.ToDictionary(t => t.Identifier, StringComparer.OrdinalIgnoreCase);
        }

        private async Task EnsureTenantsForCustomersAsync()
        {
            var existingTenantIdentifiers = new HashSet<string>(this.tenants.Tenants.Select(t => t.Identifier), StringComparer.OrdinalIgnoreCase);

            foreach (var customer in this.CustomerList)
            {
                var identifier = customer.Customer_ID.ToString();
                if (existingTenantIdentifiers.Contains(identifier))
                {
                    continue;
                }

                try
                {
                    var tenantId = Guid.NewGuid();

                    await this.Commander.Send(new CreateTenantCommand(
                        tenantId,
                        Name: customer.Customer_Desc,
                        Identifier: identifier));

                    existingTenantIdentifiers.Add(identifier);
                }
                catch (Exception ex)
                {
                    this.NotificationService.Notify(
                        NotificationSeverity.Warning,
                        $"Failed to create tenant for customer {customer.Customer_Desc}");
                }
            }
        }

        private void NavigateToTenant(ScsCustomer customer)
        {
            var identifier = customer.Customer_ID.ToString();

            if (!tenantByIdentifier.TryGetValue(identifier, out var tenant))
            {
                NotificationService.Notify(
                    NotificationSeverity.Warning,
                    "Tenant not found for this customer.");
                return;
            }

            NavigationManager.NavigateTo($"/customers/{tenant.Id}");
        }

        private void NavigateToValidations(int customer)
        {
            this.NavigationManager.NavigateTo($"/validation/{customer}");
        }
    }
}