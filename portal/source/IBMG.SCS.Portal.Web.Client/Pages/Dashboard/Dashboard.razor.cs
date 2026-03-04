// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.ApiClient;
using IBMG.SCS.Portal.Web.Client.Components.Dashboard;
using IBMG.SCS.Portal.Web.Client.Components.DonutWidget;
using IBMG.SCS.Portal.Web.Client.Components.KpiGaugeWidget;
using IBMG.SCS.Portal.Web.Client.Components.LineWidget;
using IBMG.SCS.Portal.Web.Client.Components.RankedListWidget;
using IBMG.SCS.Portal.Web.Client.Components.Widgets;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DashboardWidgetCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using IBMG.SCS.Portal.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Linq;
using static IBMG.SCS.Portal.Web.Client.Components.Widgets.WidgetsPopup;

namespace IBMG.SCS.Portal.Web.Client.Pages.Dashboard
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] public ICommander Commander { get; set; } = default!;

        [Inject] public IQuerier Querier { get; set; } = default!;

        [Inject] private DialogService DialogService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        protected bool IsEditMode { get; set; }

        private Guid _dashboardId;

        protected List<DashboardWidgetViewModel> Widgets { get; set; } = new();

        private Dictionary<string, Func<RenderFragment>> WidgetRegistry = default!;

        private List<GetAllWidgetsQueryResult.WidgetItem> AllWidgets { get; set; } = new();

        private readonly Guid _currentUserId = Guid.NewGuid();

        private const int DashboardTemplateType = 1;

        private readonly List<string> filters =
        [
            "Month To Date", "Year To Date", "Last Year", "Last Year To Date",
        ];

        private string SelectedFilter = "Year To Date";

        private List<NameAmount> SpendByOperative { get; set; } = new();

        private double? FaultyGoodsKpi { get; set; }

        private double? OtifKpi { get; set; }

        private double? InvoiceAccuracyKpi { get; set; }

        private List<CategorySpend> SpendByCategory { get; set; } = new();

        private List<RankedItem> TopProducts { get; set; } = new();

        private List<RankedItem> TopOperatives { get; set; } = new();

        private List<NameAmount> SpendByBranch { get; set; } = new();

        private List<RankedItem> TopBranches { get; set; } = new();

        [Inject]
        private DashboardDataService DashboardData { get; set; }

        private bool _isLoading = true;

        private Dictionary<int, DashboardRowLayout?> ActiveLayouts = new();

        private static readonly Dictionary<DashboardRowLayout, int[]> RowLayouts = new()
        {
            [DashboardRowLayout.OneColumn] = new[] { 12 },
            [DashboardRowLayout.TwoEqual] = new[] { 6, 6 },
            [DashboardRowLayout.ThreeEqual] = new[] { 4, 4, 4 },
            [DashboardRowLayout.FourEqual] = new[] { 3, 3, 3, 3 },
            [DashboardRowLayout.TwoUneven] = new[] { 8, 4 },
            [DashboardRowLayout.ThreeUneven] = new[] { 6, 3, 3 },
        };

        protected override async Task OnInitializedAsync()
        {
            this._isLoading = true;
            await this.LoadDashboardDataAsync();
            this.BuildWidgetRegistry();
            foreach (var row in Widgets.Select(w => w.RowOrder).Distinct())
            {
                this.SetActiveLayoutFromWidgets(row);
            }

            var allWidgetsResult = await this.Querier.Send(new GetAllWidgetsQuery());

            if (allWidgetsResult.Status == QueryResultStatus.Succeeded)
            {
                this.AllWidgets = allWidgetsResult.Data.WidgetItems.ToList();
            }
            else
            {
                return;
            }

            var dashboardsResult = await this.Querier.Send(new GetAllDashboardItemQuery());

            GetAllDashboardItemQueryResult.ToDoItem? existingDashboard = null;

            if (dashboardsResult.Status == QueryResultStatus.Succeeded)
            {
                existingDashboard = dashboardsResult.Data.ToDoItems
                    .FirstOrDefault(x =>
                        x.UserId == this._currentUserId &&
                        x.TemplateType == DashboardTemplateType);
            }

            if (existingDashboard == null)
            {
                var newDashboardId = Guid.NewGuid();

                await this.Commander.Send(new AddDashboardCommand(
                    newDashboardId,
                    this._currentUserId,
                    null,
                    DashboardTemplateType
                ));

                this._dashboardId = newDashboardId;
            }
            else
            {
                this._dashboardId = existingDashboard.Id;
            }

            var widgetsResult = await this.Querier.Send(
                new GetAllDashboardWidgetQuery());

            if (widgetsResult.Status == QueryResultStatus.Succeeded)
            {
                this.Widgets = widgetsResult.Data.Items
                    .Where(w => !w.IsRowDeleted.GetValueOrDefault())
                    .OrderBy(w => w.RowOrder)
                    .ThenBy(w => w.Position)
                    .Select(w =>
                    {
                        var widgetInfo = this.AllWidgets.FirstOrDefault(meta => meta.Id == w.WidgetId);
                        var layoutParts = w.RowLayoutType.Split(',');

                        var safeLayout =
                            layoutParts.Length >= w.Position
                                ? layoutParts[w.Position - 1]
                                : layoutParts.Last();

                        if (widgetInfo == null)
                        {
                            return new DashboardWidgetViewModel
                            {
                                DashboardWidgetId = w.Id,
                                DashboardId = w.DashboardId,
                                RowOrder = w.RowOrder,
                                Position = w.Position,
                                RowLayoutType = safeLayout,
                                Content = this.UnknownWidget($"Missing metadata for WidgetId: {w.WidgetId}"),
                            };
                        }

                        var key = widgetInfo.Name;

                        return new DashboardWidgetViewModel
                        {
                            DashboardWidgetId = w.Id,
                            DashboardId = w.DashboardId,
                            WidgetId = w.WidgetId,
                            RowOrder = w.RowOrder,
                            Position = w.Position,
                            RowLayoutType = safeLayout,
                            GroupByColumn = WidgetGroupByMap.TryGetValue(key, out var groupBy) ? groupBy : "Branch",
                            Content = WidgetRegistry.TryGetValue(key, out var factory)  ? factory() : this.UnknownWidget(key),
                            Title = widgetInfo.Name,
                        };

                    })
                    .ToList();
            }

            this._isLoading = false;
        }

        private async Task LoadDashboardDataAsync()
        {
            var normalizedFilter = this.DashboardData.NormalizeFilter(this.SelectedFilter);
            var customerId = "b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5";
            int customerDwhId = 6702124;
            this.FaultyGoodsKpi = await this.DashboardData
                .GetFaultyGoodsKpiAsync(customerId);
            this.OtifKpi = await this.DashboardData.GetOtifKpiAsync(customerId);
            this.InvoiceAccuracyKpi = await this.DashboardData.GetInvoiceAccuracyKpiAsync(customerId);
            this.SpendByOperative = await this.DashboardData.GetSpendByOperativeAsync(customerDwhId, this.SelectedFilter);
            this.SpendByCategory = await this.DashboardData.GetSpendByCategoryAsync(customerDwhId, normalizedFilter);
            this.TopProducts = await this.DashboardData.GetTopProductsBySpendAsync(customerDwhId, normalizedFilter);
            this.TopOperatives = await this.DashboardData.GetTopOperativesBySpendAsync(customerDwhId, normalizedFilter);
            this.SpendByBranch = await this.DashboardData.GetSpendByBranchAsync(customerDwhId, normalizedFilter);
            this.TopBranches = await this.DashboardData.GetTopBranchesBySpendAsync(customerDwhId, normalizedFilter);
        }



        private void SetActiveLayoutFromWidgets(int rowOrder)
        {
            var rowWidgets = Widgets
                .Where(w => w.RowOrder == rowOrder)
                .OrderBy(w => w.Position)
                .ToList();

            if (!rowWidgets.Any())
            {
                ActiveLayouts[rowOrder] = null;
                return;
            }

            var widths = rowWidgets.Select(w => w.RowLayoutType).ToList();

            ActiveLayouts[rowOrder] =
                widths.All(w => w == "12") ? DashboardRowLayout.OneColumn :
                widths.All(w => w == "6") ? DashboardRowLayout.TwoEqual :
                widths.All(w => w == "4") ? DashboardRowLayout.ThreeEqual :
                widths.All(w => w == "3") ? DashboardRowLayout.FourEqual :
                (widths.First() == "8" && widths.Skip(1).All(w => w == "4"))
            ? DashboardRowLayout.TwoUneven :
              (widths.First() == "6" && widths.Skip(1).All(w => w == "3"))
            ? DashboardRowLayout.ThreeUneven :

                null;
        }

        private bool UseWeekly(DateTime start, DateTime end)
        {
            return (end - start).TotalDays < 31;
        }

        private List<SpendPoint> GroupStaticByWeek(List<TempSpend> data)
        {
            return data
                .GroupBy(d =>
                    System.Globalization.ISOWeek.GetWeekOfYear(d.Date))
                .Select(g => new SpendPoint(
                    $"Week {g.Key}",
                    g.Sum(x => (decimal)x.Amount)
                ))
                .OrderBy(x => x.DayLabel)
                .ToList();
        }

        private List<SpendPoint> GroupStaticByMonth(List<TempSpend> data)
        {
            return data
                .GroupBy(d => new { d.Date.Year, d.Date.Month })
                .Select(g => new SpendPoint(
                    $"{g.Key.Month:D2}/{g.Key.Year}",
                    g.Sum(x => (decimal)x.Amount)
                ))
                .OrderBy(x => x.DayLabel)
                .ToList();
        }

        private List<SpendPoint> GetTotalSpendStatic(
      DateTime start,
      DateTime end)
        {
            var tempData = BuildTempSpendFromStatic();

            if (UseWeekly(start, end))
            {
                return GroupStaticByWeek(tempData);
            }

            return GroupStaticByMonth(tempData);
        }

        private async Task OnFilterChanged(object value)
        {
            this.SelectedFilter = value?.ToString() ?? this.SelectedFilter;
            this._isLoading = true;
            await this.LoadDashboardDataAsync();
            this.RebuildWidgets();
            this._isLoading = false;
            this.StateHasChanged();
        }

        protected void ToggleEditMode()
        {
            this.IsEditMode = !this.IsEditMode;
        }

        private void AddNewRow()
        {
            var nextRowOrder = this.ActiveLayouts.Any()
                ? this.ActiveLayouts.Keys.Max() + 1
                : (this.Widgets.Any() ? this.Widgets.Max(w => w.RowOrder) + 1 : 1);

            this.ActiveLayouts[nextRowOrder] = null;

            StateHasChanged();
        }

        private List<int> GetRows()
        {
            var widgetRows = this.Widgets
                .Select(w => w.RowOrder)
                .Distinct()
                .ToList();

            var layoutRows = this.ActiveLayouts.Keys.ToList();

            var rows = widgetRows
                .Union(layoutRows)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (!rows.Any())
            {
                rows.Add(1);
            }

            return rows;
        }

        private async Task AddWidget(WidgetSelection selection)
        {
            var (widgetId, _, rowOrder) = selection;

            var widgetMeta = AllWidgets.FirstOrDefault(w => w.Id == widgetId);
            if (widgetMeta == null) return;

            int layoutWidth = AutoLayoutWidth(widgetMeta.Name);

            var widgetsInRow = Widgets
                .Where(w => w.RowOrder == rowOrder)
                .ToList();

            int newPosition = widgetsInRow.Count + 1;

            var result = await Commander.Send(
                new AddDashboardWidgetCommand(
                    Guid.NewGuid(),
                    _dashboardId,
                    rowOrder,
                    layoutWidth.ToString(),
                    newPosition,
                    widgetId,
                    false
                ));

            if (result.Status != CommandResultStatus.Succeeded) return;

            Widgets.Add(new DashboardWidgetViewModel
            {
                DashboardWidgetId = result.Data.Id,
                WidgetId = widgetId,
                RowOrder = rowOrder,
                Position = newPosition,
                RowLayoutType = layoutWidth.ToString(),
                Content = WidgetRegistry[widgetMeta.Name]()
            });

            StateHasChanged();
        }

        public async Task DeleteGroup(int rowOrder)
        {
            var confirm = await this.DialogService.Confirm(
                "Are you sure you want to delete this row and all its widgets?",
                "Delete Row",
                new ConfirmOptions
                {
                    OkButtonText = "Delete",
                    CancelButtonText = "Cancel",
                });

            if (confirm != true)
            {
                return;
            }

            var widgetsInRow = this.Widgets
                .Where(w => w.RowOrder == rowOrder)
                .ToList();

            if (!widgetsInRow.Any())
            {
                return;
            }

            foreach (var w in widgetsInRow)
            {
                await this.Commander.Send(new DeleteDashboardWidgetCommand(w.DashboardWidgetId));
            }

            this.Widgets.RemoveAll(w => w.RowOrder == rowOrder);

            this.StateHasChanged();
        }

        private (DateTime Start, DateTime End) ResolveDateRange(string filter)
        {
            var today = DateTime.Today;

            return filter switch
            {
                "Month To Date" => (new DateTime(today.Year, today.Month, 1), today),

                "Year To Date" => (new DateTime(today.Year, 1, 1), today),

                "Last Year" =>
                    (new DateTime(today.Year - 1, 1, 1),
                     new DateTime(today.Year - 1, 12, 31)),

                "Last Year To Date" =>
                    (new DateTime(today.Year - 1, 1, 1),
                     today.AddYears(-1)),

                _ => (new DateTime(today.Year, today.Month, 1), today)
            };
        }

        private List<TempSpend> BuildTempSpendFromStatic()
        {
            var today = DateTime.Today;

            return new List<SpendPoint>
    {
        new("1", 720),
        new("7", 320),
        new("14", 280),
        new("21", 460),
        new("28", 580),
    }
            .Select(p => new TempSpend
            {
                Date = new DateTime(today.Year, today.Month, int.Parse(p.DayLabel)),
                Amount = p.Amount
            })
            .ToList();
        }

        private void BuildWidgetRegistry()
        {
            var range = ResolveDateRange(this.SelectedFilter);
            var startDate = range.Start;
            var endDate = range.End;

            var totalSpendPoints = GetTotalSpendStatic(startDate, endDate);

            this.WidgetRegistry = new()
            {
                ["TotalSpend"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(LineWidget<SpendPoint>));
                    builder.AddAttribute(1, "Title", "Total Spend");
                    builder.AddAttribute(2, "Data", totalSpendPoints);
                    builder.AddAttribute(3, "Category", "DayLabel");
                    builder.AddAttribute(4, "Value", "Amount");
                    builder.AddAttribute(5, "Format", "{0:C0}");
                    builder.AddAttribute(6, "ShowLegend", true);
                    builder.CloseComponent();
                },

                ["SpendByCategory"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(DonutWidget<CategorySpend>));
                    builder.AddAttribute(1, "Title", "Spend by Category");
                    builder.AddAttribute(2, "Data", this.SpendByCategory);
                    builder.AddAttribute(3, "Category", "Cat");
                    builder.AddAttribute(4, "Value", "Amount");
                    builder.AddAttribute(5, "ShowLegend", true);
                    builder.CloseComponent();
                },

                ["TopProductsBySpend"] = () => builder =>
              {
                  builder.OpenComponent(0, typeof(RankedListWidget<RankedItem>));
                  builder.AddAttribute(1, "Title", "Top Products by £ Spend");
                  builder.AddAttribute(2, "Items", this.TopProducts);  
                  builder.AddAttribute(3, "RankField", (Func<RankedItem, int>)(i => i.Rank));
                  builder.AddAttribute(4, "NameField", (Func<RankedItem, string>)(i => i.Name));
                  builder.AddAttribute(5, "AmountField", (Func<RankedItem, decimal>)(i => i.Amount));
                  builder.CloseComponent();
                },

                ["SpendByOperative"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(DonutWidget<NameAmount>));
                    builder.AddAttribute(1, "Title", "Spend by Operative");
                    builder.AddAttribute(2, "Data", this.SpendByOperative); 
                    builder.AddAttribute(3, "Category", "Text");
                    builder.AddAttribute(4, "Value", "Amount");
                    builder.AddAttribute(5, "ShowLegend", true);
                    builder.CloseComponent();
                },

                ["TopOperativesBySpend"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(RankedListWidget<RankedItem>));
                    builder.AddAttribute(1, "Title", "Top Operatives by Spend");
                    builder.AddAttribute(2, "Items", this.TopOperatives);   
                    builder.AddAttribute(3, "RankField", (Func<RankedItem, int>)(i => i.Rank));
                    builder.AddAttribute(4, "NameField", (Func<RankedItem, string>)(i => i.Name));
                    builder.AddAttribute(5, "AmountField", (Func<RankedItem, decimal>)(i => i.Amount));
                    builder.CloseComponent();
                },

                ["SpendByBranch"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(DonutWidget<NameAmount>));
                    builder.AddAttribute(1, "Title", "Spend by Branch");
                    builder.AddAttribute(2, "Data", this.SpendByBranch); 
                    builder.AddAttribute(3, "Category", "Text");
                    builder.AddAttribute(4, "Value", "Amount");
                    builder.AddAttribute(5, "ShowLegend", true);
                    builder.CloseComponent();
                },

                ["TopBranchesBySpend"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(RankedListWidget<RankedItem>));
                    builder.AddAttribute(1, "Title", "Top Branches by Spend");
                    builder.AddAttribute(2, "Items", this.TopBranches);
                    builder.AddAttribute(3, "RankField", (Func<RankedItem, int>)(i => i.Rank));
                    builder.AddAttribute(4, "NameField", (Func<RankedItem, string>)(i => i.Name));
                    builder.AddAttribute(5, "AmountField", (Func<RankedItem, decimal>)(i => i.Amount));
                    builder.CloseComponent();
                },

                ["FaultyGoodsKpi"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(KpiGaugeWidget));
                    builder.AddAttribute(1, "Title", "Faulty Goods Report KPI");
                    builder.AddAttribute(2, "Description", "Faulty Goods Reports");
                    builder.AddAttribute(3, "Value", this.FaultyGoodsKpi ?? 0);
                    builder.CloseComponent();
                },

                ["OtifKpi"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(KpiGaugeWidget));
                    builder.AddAttribute(1, "Title", "OTIF KPI");
                    builder.AddAttribute(2, "Description", "Delivered On-Time-in-Full");
                    builder.AddAttribute(3, "Value", this.OtifKpi ?? 0); 
                    builder.CloseComponent();
                },

                ["InvoiceAccuracyKpi"] = () => builder =>
                {
                    builder.OpenComponent(0, typeof(KpiGaugeWidget));
                    builder.AddAttribute(1, "Title", "Invoice Accuracy KPI");
                    builder.AddAttribute(2, "Description", "Invoice Accuracy");
                    builder.AddAttribute(3, "Value", this.InvoiceAccuracyKpi ?? 0);
                    builder.CloseComponent();
                },
            };
        }

        private RenderFragment UnknownWidget(string key) => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "alert alert-warning");
            builder.AddContent(2, $"Unknown widget: {key}");
            builder.CloseElement();
        };

        private async Task ApplyRowLayout(int rowOrder, DashboardRowLayout layout)
        {
            if (!RowLayouts.TryGetValue(layout, out var columns))
            {
                return;
            }

            var widgetsInRow = this.Widgets
                .Where(w => w.RowOrder == rowOrder)
                .OrderBy(w => w.Position)
                .ToList();

            if (!widgetsInRow.Any())
            {
                return;
            }

            for (int i = 0; i < widgetsInRow.Count; i++)
            {
                widgetsInRow[i].RowLayoutType =
                    i < columns.Length
                        ? columns[i].ToString()
                        : columns.Last().ToString();

                await this.Commander.Send(
                    new UpdateDashboardWidgetCommand(
                        widgetsInRow[i].DashboardWidgetId,
                        rowOrder,
                        string.Join(",", columns)
                    )
                );
            }

            this.ActiveLayouts[rowOrder] = layout;

            this.StateHasChanged();
        }

        private void RebuildWidgets()
        {
            this.BuildWidgetRegistry();

            for (int i = 0; i < this.Widgets.Count; i++)
            {
                var widget = this.Widgets[i];

                var meta = this.AllWidgets.FirstOrDefault(x => x.Id == widget.WidgetId);
                if (meta == null)
                {
                    continue;
                }

                if (this.WidgetRegistry.TryGetValue(meta.Name, out var factory))
                {
                    widget.Content = factory();
                }
                else
                {
                    widget.Content = this.UnknownWidget(meta.Name);
                }
            }
        }

        private int AutoLayoutWidth(string widgetName)
        {
            if (widgetName.Contains("Spend"))
            {
                return 4;
            }

            if (widgetName.Contains("Kpi"))
            {
                return 4;
            }

            return 6;
        }

        private static readonly Dictionary<string, string> WidgetGroupByMap = new()
        {
            ["SpendByOperative"] = "Operative",
            ["TopOperativesBySpend"] = "Operative",

            ["SpendByBranch"] = "Branch",
            ["TopBranchesBySpend"] = "Branch",
            ["SpendByCategory"] = "Category",
            ["TotalSpend"] = "Branch",
            ["FaultyGoodsKpi"] = "Branch",
            ["OtifKpi"] = "Branch",
            ["InvoiceAccuracyKpi"] = "Branch",
        };

        private async Task NavigateToOrders(DashboardWidgetViewModel widget)
        {
            await Task.Delay(30);

            var period = Uri.EscapeDataString(this.SelectedFilter);
            var groupBy = Uri.EscapeDataString(widget.GroupByColumn);

            this.Navigation.NavigateTo($"/orders-grid/{period}/{groupBy}");
        }

    }

    public class TempSpend
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

}