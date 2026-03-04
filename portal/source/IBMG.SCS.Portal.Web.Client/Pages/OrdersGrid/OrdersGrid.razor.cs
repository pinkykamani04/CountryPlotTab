// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.ApiClient;
using IBMG.SCS.Portal.Web.Client.Components.DateRangeDialog;
using IBMG.SCS.Portal.Web.Client.Components.OrdersDialog;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Radzen;
using Radzen.Blazor;

namespace IBMG.SCS.Portal.Web.Client.Pages.OrdersGrid
{
    public partial class OrdersGrid
    {
        [Parameter]
        public string? Period { get; set; }

        [Parameter]
        public string? GroupBy { get; set; }

        [Inject]
        private IClient Client { get; set; } = default!;

        private DateTime startDate = new(2025, 1, 10);
        private DateTime endDate = new(2025, 2, 15);
        private string searchText = string.Empty;
        private string groupBy = "Days";
        private List<string> GroupOptions = new() { "Days", "Weeks", "Months" };
        private string groupByColumn = "Operative";
        private string orderBy = "Operative";

        private string? selectedFilterType;
        private string? activeFilterValue;
        RadzenDataGrid<DisplayRow> TimesheetGrid;

        private ClickState lastClicked = null;

        private List<DisplayRow> displayRows = new();
        private List<DateTime> columnDates = new();
        private List<OperativeTimesheet> RawData = new();
        private List<OrderDetails> OrdersList = new();
        private List<string> FilterTypeOptions = new()
                                             {
                                               "Operative",
                                               "Transaction Type",
                                               "Branch",
                                               "Category",
                                             };

        private List<string> OrderByOptions = new()
                                             {
                                                "Operative",
                                                "Total Spent",
                                                "Branch",
                                                "Category",
                                                "Transaction Type",
                                             };

        private bool IsGrouped => !string.IsNullOrWhiteSpace(this.GroupBy);

        private bool ShowOperative => !this.IsGrouped || this.GroupBy == "Operative";

        private bool ShowBranch => !this.IsGrouped || this.GroupBy == "Branch";

        private bool ShowCategory => !this.IsGrouped || this.GroupBy == "Category";

        private bool ShowTransactionType => !this.IsGrouped || this.GroupBy == "Transaction Type";

        private bool _periodAppliedFromQuery = false;

        private decimal SalesValue { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (!this._periodAppliedFromQuery)
            {
                if (!string.IsNullOrWhiteSpace(this.Period))
                {
                    var (from, to) = ResolvePeriod(this.Period);
                    this.startDate = from;
                    this.endDate = to;
                }

                this.groupByColumn = !string.IsNullOrWhiteSpace(this.GroupBy)
                    ? this.GroupBy
                    : "Operative";

                this._periodAppliedFromQuery = true;
            }

            await this.LoadOrdersFromApi();
            this.BuildRows();
        }

        private static (DateTime From, DateTime To) ResolvePeriod(string? period)
        {
            var today = DateTime.UtcNow.Date;

            return period switch
            {
                "Month To Date" =>
                    (new DateTime(today.Year, today.Month, 1), today),

                "Year To Date" =>
                    (new DateTime(today.Year, 1, 1), today),

                "Last Year" =>
                    (new DateTime(today.Year - 1, 1, 1),
                     new DateTime(today.Year - 1, 12, 31)),

                "Last Year To Date" =>
                    (new DateTime(today.Year - 1, 1, 1),
                     today.AddYears(-1)),

                _ =>
                    (DateTime.MinValue, DateTime.MaxValue),
            };
        }

        private async Task OnFilterTypeChanged(object value)
        {
            this.selectedFilterType = value?.ToString();
            this.activeFilterValue = null;

            await this.InvokeAsync(this.StateHasChanged);
        }

        private async Task OnOrderByChanged(object value)
        {
            this.orderBy = value?.ToString();
            await this.LoadOrdersFromApi();
        }

        private async Task ResetFilter()
        {
            this.startDate = new(2025, 1, 10);
            this.endDate = new(2025, 2, 15);
            this.selectedFilterType = null;
            this.activeFilterValue = null;
            this.groupBy = "Days";

            await this.LoadOrdersFromApi();
        }

        private async Task LoadOrdersFromApi()
        {
            var result = await this.Client.ListCustomerOrdersAsync(
                6702124,
                this.startDate,
                this.endDate,
                this.activeFilterValue,
                this.orderBy,
                this.GroupBy
            );

            if (result?.Data is not JArray array)
            {
                this.OrdersList.Clear();
                this.RawData.Clear();
                this.displayRows.Clear();
                return;
            }

            var orders = array.ToObject<List<dynamic>>() ?? new();
            this.BuildUiModels(orders);
        }

        private void BuildUiModels(List<dynamic> orders)
        {
            this.OrdersList = orders.Select(o =>
            {
                var date =
                    o.transaction_Date ??
                    o.date;

                var total =
                    o.invoiced_Sales_Total ??
                    o.total;

                var groupKey = this.IsGrouped && o.groupKey != null
                             ? o.groupKey.ToString()
                             : null;

                return new OrderDetails
                {
                    Date = date != null
                        ? DateTime.Parse(date.ToString()).Date
                        : DateTime.MinValue,

                    Operative = !this.IsGrouped || this.GroupBy == "Operative"
                        ? o.operative_Desc ?? o.operative ?? "Unassigned"
                        : string.Empty,

                    Branch = this.IsGrouped && this.GroupBy == "Branch"
                      ? groupKey
                       : o.branch_Desc ?? o.branch ?? string.Empty,

                    Category = this.IsGrouped && this.GroupBy == "Category"
                        ? groupKey
                        : o.product_Category_Desc ?? o.category ?? string.Empty,

                    TransactionType = this.IsGrouped && this.GroupBy == "Transaction Type"
                        ? groupKey
                        : o.order_Category ?? o.transactionType ?? string.Empty,

                    Total = total != null
                        ? (decimal)total
                        : 0m,
                };
            }).ToList();

            this.SalesValue = this.OrdersList.Sum(o => o.Total);

            if (this.IsGrouped)
            {
                this.RawData = this.OrdersList.Select(o => new OperativeTimesheet
                {
                    Operative = o.Operative,
                    Branch = o.Branch,
                    Category = o.Category,
                    TransactionType = o.TransactionType,

                    TotalSpent = o.Total,
                    DailyValues = new Dictionary<DateTime, decimal>
            {
                { o.Date, o.Total },
            },
                }).ToList();
            }
            else
            {
                this.RawData = this.OrdersList.Select(o => new OperativeTimesheet
                {
                    Operative = o.Operative,
                    Branch = o.Branch,
                    Category = o.Category,
                    TransactionType = o.TransactionType,

                    TotalSpent = o.Total,
                    DailyValues = new Dictionary<DateTime, decimal>
            {
                { o.Date, o.Total },
            },
                }).ToList();
            }
        }

        private void BuildColumnDates()
        {
            if (this.groupBy == "Days")
            {
                this.columnDates = Enumerable.Range(0, (this.endDate - this.startDate).Days + 1)
                    .Select(i => startDate.Date.AddDays(i))
                    .ToList();
            }
            else if (this.groupBy == "Weeks")
            {
                this.columnDates = this.BuildWeeks(this.startDate, this.endDate).Select(w => w.Start).ToList();
            }
            else
            {
                this.columnDates = this.BuildMonths(this.startDate, this.endDate);
            }
        }

        private decimal GetWeekValue(OperativeTimesheet op, DateTime dt)
        {
            var weeks = BuildWeeks(this.startDate, this.endDate);
            var wk = weeks[this.columnDates.IndexOf(dt)];

            return op.DailyValues
                .Where(x => x.Key >= wk.Start && x.Key <= wk.End)
                .Sum(x => x.Value);
        }

        private decimal GetMonthValue(OperativeTimesheet op, DateTime dt)
        {
            return op.DailyValues
                .Where(x => x.Key.Month == dt.Month && x.Key.Year == dt.Year)
                .Sum(x => x.Value);
        }

        private List<WeekRange> BuildWeeks(DateTime start, DateTime end)
        {
            var list = new List<WeekRange>();
            var cur = start.AddDays(-(int)start.DayOfWeek + (int)DayOfWeek.Monday);

            while (cur <= end)
            {
                list.Add(new()
                {
                    Start = cur < start ? start : cur,
                    End = cur.AddDays(6) > end ? end : cur.AddDays(6),
                });

                cur = cur.AddDays(7);
            }

            return list;
        }

        private List<DateTime> BuildMonths(DateTime start, DateTime end)
        {
            var list = new List<DateTime>();
            var cur = new DateTime(start.Year, start.Month, 1);

            while (cur <= end)
            {
                list.Add(cur);
                cur = cur.AddMonths(1);
            }

            return list;
        }

        private void BuildRows()
        {
            this.BuildColumnDates();

            var rows = new List<DisplayRow>();

            foreach (var op in this.RawData)
            {
                var row = new DisplayRow
                {
                    Operative = this.ShowOperative ? op.Operative : string.Empty,
                    Branch = this.ShowBranch ? op.Branch : string.Empty,
                    Category = this.ShowCategory ? op.Category : string.Empty,
                    TransactionType = this.ShowTransactionType ? op.TransactionType : string.Empty,
                    TotalSpent = op.TotalSpent.GetValueOrDefault(),
                    Columns = new(),
                };

                foreach (var dt in this.columnDates)
                {
                    decimal value = groupBy switch
                    {
                        "Weeks" => this.GetWeekValue(op, dt),
                        "Months" => this.GetMonthValue(op, dt),
                        _ => op.DailyValues.GetValueOrDefault(dt),
                    };

                    row.Columns[dt] = value;
                }

                rows.Add(row);
            }

            this.displayRows = rows;
            this.StateHasChanged();
        }

        private async Task OnFilterInput(ChangeEventArgs e)
        {
            this.activeFilterValue = e.Value?.ToString();

            await this.LoadOrdersFromApi();
        }

        private void ApplyFilter()
        {
            this.BuildRows();
        }

        private (int RowIndex, DateTime ColumnDate)? selectedCell;

        private void OnCellClick(DisplayRow row, DateTime dt)
        {
            var rowIndex = displayRows.IndexOf(row);

            this.selectedCell = (rowIndex, dt.Date);

            this.StateHasChanged();

            this.OpenOrdersModal(row.Operative, dt);
        }

        private string GetCalculatedCssClass(DisplayRow row, DateTime dt)
        {
            if (this.selectedCell == null)
            {
                return string.Empty;
            }

            var rowIndex = displayRows.IndexOf(row);

            return this.selectedCell.Value.RowIndex == rowIndex
                   && this.selectedCell.Value.ColumnDate == dt.Date
                ? "clicked-cell"
                : string.Empty;
        }

        private string FormatHeader(DateTime dt) =>
      this.groupBy switch
      {
          "Days" => $"{dt:dd MMM}<br/>({dt:ddd})",
          "Weeks" => $"{dt:dd MMM} - {dt.AddDays(6):dd MMM}",
          "Months" => dt.ToString("MMM yyyy"),
          _ => dt.ToShortDateString(),
      };


        private async Task OpenDateDialog()
        {
            var parameters = new Dictionary<string, object>
    {
        { "InitialFrom", this.startDate },
        { "InitialTo", this.endDate },
        {
            "ApplyCallback",
            new Func<DateTime, DateTime, Task>(async (f, t) =>
            {
                this.startDate = f;
                this.endDate = t;

                await this.LoadOrdersFromApi();
                this.BuildRows();
            })
        },
    };

            await this.DialogService.OpenAsync<DateRangeDialog>(
                "Select Period",
                parameters,
                new DialogOptions
                {
                    Height = "auto",
                    Width = "auto",
                    ShowClose = true,
                });
        }

        private void OpenOrdersModal(string operative, DateTime dt)
        {
            this.DialogService.Open<OrdersDialog>(
                "Orders",
                new Dictionary<string, object>
                {
            { "Operative", operative },
            { "Date", dt },
            { "GroupBy", this.GroupBy },
                },
                new DialogOptions
                {
                    Width = "650px",
                    Height = "450px",
                    ShowClose = true,
                });
        }

        private async Task ExportCsv()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("Operative,Total Spent,");
            sb.Append(string.Join(",", this.columnDates.Select(d => $"\"{this.FormatHeader(d)}\"")));
            sb.AppendLine();

            foreach (var r in this.displayRows)
            {
                sb.Append($"\"{r.Operative}\",{r.TotalSpent},");

                var cols = this.columnDates.Select(dt => r.Columns.GetValueOrDefault(dt).ToString("F2"));
                sb.Append(string.Join(",", cols.Select(c => $"\"{c}\"")));
                sb.AppendLine();
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            await this.JS.InvokeVoidAsync("downloadFileFromBytes", "Timesheet.csv", bytes);
        }

        private async Task ExportPdf()
        {
            var html = await this.JS.InvokeAsync<string>("getOuterHtml", "#timesheet-export-table");
            await this.JS.InvokeVoidAsync("openPrintWindow", html);
        }

        private bool showDownloadMenu = false;

        private void ToggleDownloadMenu()
        {
            this.showDownloadMenu = !this.showDownloadMenu;
        }

        private async Task DownloadPdf()
        {
            this.showDownloadMenu = false;
            await this.ExportPdf();
        }

        private async Task DownloadCsv()
        {
            this.showDownloadMenu = false;
            await this.ExportCsv();
        }

    }
}