// Copyright (c) IBMG. All rights reserved.

using System.Text.Json;
using IBMG.SCS.Portal.ApiClient;
using IBMG.SCS.Portal.Web.Client.Models;
using IBMG.SCS.Portal.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.Widgets
{
    public partial class WidgetsPopup : ComponentBase
    {
        [Parameter] public EventCallback<WidgetSelection> OnWidgetSelected { get; set; }

        [Parameter]
        public int RowOrder { get; set; }

        [Inject]
        private DialogService DialogService { get; set; }

        private List<WidgetsModel>? AvailableWidgets;

        public record SpendPoint(string DayLabel, decimal Amount);

        public record CategorySpend(string Cat, decimal Amount);

        public record RankedItem(int Rank, string Name, decimal Amount);

        [Inject]
        private IClient client { get; set; }

        [Inject]
        private KerridgeBranchService KerridgeBranchService { get; set; }

        [Inject]
        private DashboardDataService DashboardData { get; set; }

        private double? FaultyGoodsKpi { get; set; }

        private double? OtifKpi { get; set; }

        private double? InvoiceAccuracyKpi { get; set; }

        private List<NameAmount> SpendByOperative { get; set; } = new();

        private List<CategorySpend> spendPoints;

        private List<RankedItem> TopOperatives { get; set; } = new();

        private List<NameAmount> SpendByBranch { get; set; } = new();

        private List<RankedItem> TopBranches { get; set; } = new();

        private List<RankedItem> TopProducts { get; set; } = new();

        private bool isLoading {  get; set; } = false;

        private int CustomerId = 6702124;

        private string selectedFilter = "Year To Date";

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var assembly = typeof(WidgetsPopup).Assembly;
            var resourceName =
                "IBMG.SCS.Portal.Web.Client.Data.widgets.json";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            this.AvailableWidgets =
                JsonSerializer.Deserialize<List<WidgetsModel>>(json) ?? new();
            this.FaultyGoodsKpi = await this.DashboardData.GetFaultyGoodsKpiAsync("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5");
            this.OtifKpi = await this.DashboardData.GetOtifKpiAsync("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5");
            this.InvoiceAccuracyKpi = await this.DashboardData.GetInvoiceAccuracyKpiAsync("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5");
            this.spendPoints = await this.DashboardData.GetSpendByCategoryAsync(this.CustomerId, string.Empty);
            this.TopOperatives = await this.DashboardData.GetTopOperativesBySpendAsync(this.CustomerId, string.Empty);
            this.SpendByBranch = await this.DashboardData.GetSpendByBranchAsync(this.CustomerId, string.Empty);
            this.TopBranches = await this.DashboardData.GetTopBranchesBySpendAsync(this.CustomerId, string.Empty);
            this.TopProducts = await this.DashboardData.GetTopProductsBySpendAsync(this.CustomerId, string.Empty);
            this.isLoading = false;
        }

        private async Task SelectWidget(Guid id, string name)
        {
            if (this.OnWidgetSelected.HasDelegate)
            {
                await this.OnWidgetSelected.InvokeAsync(
                    new WidgetSelection(id, name, this.RowOrder)
                );
            }

            this.DialogService.Close();
        }

        List<SpendPoint> TotalSpendPoints = new() { new("1", 720), new("7", 320), new("14", 280), new("21", 460), new("28", 580) };
    }
}