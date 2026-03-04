using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using Radzen.Blazor;

namespace IBMG.SCS.Portal.Web.Client.Pages
{
    public partial class ReportsAnalyticsBase : ComponentBase
    {
        [Inject]
        protected IQuerier Querier { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected int selectedIndex = 0;

        protected double AchievedPercentage;
        protected double RemainingPercentage;
        protected Dictionary<int, List<ReportingTransactionDto>> TabTransactions = new()
        {
            { 0, new List<ReportingTransactionDto>() },
            { 1, new List<ReportingTransactionDto>() },
            { 2, new List<ReportingTransactionDto>() },
        };

        protected List<MonthlyMetricDto> monthlyChartData = new();
        protected List<MonthlyMetricDto> transactionCountChartData = new();
        protected List<MonthlyMetricDto> totalValueChartData = new();
        protected List<MonthlyMetricDto> monthlyAverageCostData = new();
        protected List<MonthlyMetricDto> tonnageRevenueData = new();
        protected List<MonthlyMetricDto> lastYearToDateData = new();
        protected List<MonthlyMetricDto> lastFullYearData = new();
        protected List<StreamPerformanceDto> streamPerformanceData = new();
        protected List<YearlyComparisonDto> wasteProcessingCostData = new();

        protected decimal totalStreamValue = 0;
        protected List<MonthlyMetricDto> totalRevenueChartData = new();

        public bool showCustomerSiteDropdown = false;
        public bool showDateRangeDropdown = false;
        public bool showWasteStreamDropdown = false;
        public bool showContainerTypeDropdown = false;
        protected List<LookupItemDto> customerSites = new();
        protected List<LookupItemDto> wasteStreams = new();
        protected List<LookupItemDto> containerTypes = new();
        protected decimal TotalTonnageTransactionCount;
        public RadzenDataGrid<ReportingTransactionDto> grid;
        public int pageSize = 10;
        public int currentPage = 0;
        public int totalCount = 0;

        public int totalPages => totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;

        public List<int> pageSizeOptions = new List<int> { 5, 10, 20, 50, 100 };

        public List<ReportingTransactionDto> allTransactions = new List<ReportingTransactionDto>();

        public void ToggleDropdown(string dropdownName)
        {
            showCustomerSiteDropdown = dropdownName == "customerSite" ? !showCustomerSiteDropdown : false;
            showDateRangeDropdown = dropdownName == "dateRange" ? !showDateRangeDropdown : false;
            showWasteStreamDropdown = dropdownName == "wasteStream" ? !showWasteStreamDropdown : false;
            showContainerTypeDropdown = dropdownName == "containerType" ? !showContainerTypeDropdown : false;
        }

        public string GetCustomerSiteText() =>
         CurrentFilters.CustomerSiteId.HasValue
             ? customerSites.FirstOrDefault(s => s.Id == CurrentFilters.CustomerSiteId.Value)?.Name ?? "Customer Site"
             : "Customer Site";

        public string GetWasteStreamText() =>
          CurrentFilters.WasteStreamId.HasValue
              ? wasteStreams.FirstOrDefault(s => s.Id == CurrentFilters.WasteStreamId.Value)?.Name ?? "Waste Stream"
              : "Waste Stream";

        public string GetContainerTypeText() =>
          CurrentFilters.ContainerTypeId.HasValue
              ? containerTypes.FirstOrDefault(t => t.Id == CurrentFilters.ContainerTypeId.Value)?.Name ?? "Container Type"
              : "Container Type";

        protected string DateRangeDisplay => CurrentFilters.StartDate.HasValue && CurrentFilters.EndDate.HasValue
            ? $"{CurrentFilters.StartDate:dd/MM/yyyy} To {CurrentFilters.EndDate:dd/MM/yyyy}"
            : "Select Date Range";

        public List<ReportingTransactionDto> GetFilteredTransactions()
        {
            var filter = CurrentFilters;

            var transactionsForTab = TabTransactions.ContainsKey(selectedIndex)
                ? TabTransactions[selectedIndex]
                : new List<ReportingTransactionDto>();

            return transactionsForTab
                .Where(t =>
                    (!filter.CustomerSiteId.HasValue || t.CustomerSiteId == filter.CustomerSiteId) &&
                    (!filter.WasteStreamId.HasValue || t.WasteStreamId == filter.WasteStreamId) &&
                    (!filter.ContainerTypeId.HasValue || t.ContainerTypeId == filter.ContainerTypeId) &&
                    (!filter.StartDate.HasValue || t.TransactionDate.Date >= filter.StartDate.Value.Date) &&
                    (!filter.EndDate.HasValue || t.TransactionDate.Date <= filter.EndDate.Value.Date))
                .ToList();
        }

        protected Dictionary<int, ReportFilterState> TabFilters = new()
        {
            { 0, new ReportFilterState() },
            { 1, new ReportFilterState() },
            { 2, new ReportFilterState() },
        };

        protected ReportFilterState CurrentFilters => TabFilters[selectedIndex];

        public bool IsShortRange()
        {
            if (!CurrentFilters.StartDate.HasValue || !CurrentFilters.EndDate.HasValue)
                return false;

            return (CurrentFilters.EndDate.Value - CurrentFilters.StartDate.Value).TotalDays <= 31;
        }

        protected async Task LoadDropdownsAsync()
        {
            var customerResult = await Querier.Send(new GetCustomerSitesQuery());
            if (customerResult.Status == BluQube.Constants.QueryResultStatus.Succeeded)
            {
                customerSites = customerResult.Data.Items.ToList();
            }

            var wasteResult = await Querier.Send(new GetWasteStreamsQuery());
            if (wasteResult.Status == BluQube.Constants.QueryResultStatus.Succeeded)
            {
                wasteStreams = wasteResult.Data.Items.ToList();
            }

            var containerResult = await Querier.Send(new GetContainerTypesQuery());
            if (containerResult.Status == BluQube.Constants.QueryResultStatus.Succeeded)
            {
                containerTypes = containerResult.Data.Items.ToList();
            }
        }

        public string GetDateRangeDisplay()
        {
            if (CurrentFilters.StartDate.HasValue && CurrentFilters.EndDate.HasValue)
            {
                return $"{CurrentFilters.StartDate:dd/MM/yyyy} To {CurrentFilters.EndDate:dd/MM/yyyy}";
            }
            return "Select Date Range";
        }

        public async Task SelectCustomerSite(int id)
        {
            CurrentFilters.CustomerSiteId =
                CurrentFilters.CustomerSiteId == id ? null : id;

            showCustomerSiteDropdown = false;
            await LoadTransactionsAsync();
        }

        protected void BuildTotalRevenueValueChart()
        {
            var data = GetFilteredTransactions();
            if (!data.Any())
            {
                totalRevenueChartData = new();
                return;
            }

            if (IsShortRange())
            {
                totalRevenueChartData = data
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key.Day,
                        Month = g.Key.ToString("dd MMM"),
                        Value = g.Sum(x => x.TransactionValue)
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
            else
            {
                totalRevenueChartData = data
                    .GroupBy(t => t.TransactionDate.Month)
                    .Select(g =>
                    {
                        int monthNumber = g.Key;
                        int year = g.First().TransactionDate.Year;
                        string monthName;

                        try
                        {
                            if (monthNumber >= 1 && monthNumber <= 12)
                            {
                                monthName = new DateTime(year, monthNumber, 1).ToString("MMM");
                            }
                            else
                            {
                                monthName = "Unknown";
                            }
                        }
                        catch
                        {
                            monthName = "Unknown";
                        }

                        return new MonthlyMetricDto
                        {
                            MonthNumber = monthNumber,
                            Month = monthName,
                            Value = g.Sum(x => x.TransactionValue),
                        };
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }

            if (totalRevenueChartData.Count == 1)
            {
                var point = totalRevenueChartData[0];
                totalRevenueChartData.Add(new MonthlyMetricDto
                {
                    MonthNumber = point.MonthNumber + 1,
                    Month = point.Month + " ",
                    Value = point.Value,
                });
            }
        }

        public async Task SelectWasteStream(int id)
        {
            CurrentFilters.WasteStreamId =
                CurrentFilters.WasteStreamId == id ? null : id;

            showWasteStreamDropdown = false;
            await LoadTransactionsAsync();
        }

        public async Task SelectContainerType(int id)
        {
            CurrentFilters.ContainerTypeId =
                CurrentFilters.ContainerTypeId == id ? null : id;

            showContainerTypeDropdown = false;
            await LoadTransactionsAsync();
        }

        public void SelectLastWeek()
        {
            CurrentFilters.EndDate = DateTime.Today;
            CurrentFilters.StartDate = DateTime.Today.AddDays(-7);
        }

        public void SelectThisWeek()
        {
            var today = DateTime.Today;
            CurrentFilters.StartDate = today.AddDays(-(int)today.DayOfWeek);
            CurrentFilters.EndDate = CurrentFilters.StartDate.Value.AddDays(6);
        }

        public void SelectThisMonth()
        {
            var today = DateTime.Today;
            CurrentFilters.StartDate = new DateTime(today.Year, today.Month, 1);
            CurrentFilters.EndDate = CurrentFilters.StartDate.Value.AddMonths(1).AddDays(-1);
        }

        public void SelectThisYear()
        {
            var today = DateTime.Today;
            CurrentFilters.StartDate = new DateTime(today.Year, 1, 1);
            CurrentFilters.EndDate = new DateTime(today.Year, 12, 31);
        }

        public void ClearDates()
        {
            CurrentFilters.StartDate = null;
            CurrentFilters.EndDate = null;
        }

        public async Task ApplyDates()
        {
            showDateRangeDropdown = false;
            await LoadTransactionsAsync();
            StateHasChanged();
        }

        public async Task ResetFilters()
        {
            TabFilters[selectedIndex] = new ReportFilterState();

            showCustomerSiteDropdown = false;
            showDateRangeDropdown = false;
            showWasteStreamDropdown = false;
            showContainerTypeDropdown = false;

            await LoadTransactionsAsync();
        }

        protected void BuildPerformanceDonut()
        {
            var today = DateTime.Today;
            var currentYear = today.Year;
            var f = CurrentFilters;

            var filteredTransactions = GetFilteredTransactions()
              .Where(t => t.TonnageTransaction > 0)
              .ToList();

            DateTime effectiveStartDate = f.StartDate ?? new DateTime(currentYear, 1, 1);
            DateTime effectiveEndDate = f.EndDate ?? new DateTime(currentYear, 12, 31);

            filteredTransactions = filteredTransactions
                .Where(t => t.TransactionDate.Date >= effectiveStartDate.Date &&
                            t.TransactionDate.Date <= effectiveEndDate.Date)
                .ToList();

            var achievedTonnage = filteredTransactions
                .Where(t => t.TransactionDate.Date <= today)
                .Sum(t => t.NetWeight);

            var futureTonnage = filteredTransactions
                .Where(t => t.TransactionDate.Date > today)
                .Sum(t => t.NetWeight);

            var totalTonnage = achievedTonnage + futureTonnage;

            if (totalTonnage <= 0)
            {
                AchievedPercentage = 0;
                RemainingPercentage = 100;
                totalStreamValue = 0;
                return;
            }

            AchievedPercentage = Math.Round((double)(achievedTonnage / totalTonnage) * 100, 2);
            RemainingPercentage = Math.Round((double)(futureTonnage / totalTonnage) * 100, 2);

            totalStreamValue = totalTonnage;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownsAsync();
            await LoadTransactionsAsync();
            LoadData();
            NavigationManager.LocationChanged += (sender, args) =>
            {
                InvokeAsync(StateHasChanged);
            };
        }

        private void LoadData()
        {
            allTransactions = GetFilteredTransactions()?.ToList() ?? new List<ReportingTransactionDto>();
            totalCount = allTransactions.Count;
            currentPage = 0;

        }

        public IEnumerable<ReportingTransactionDto> GetCurrentPageData()
        {
            if (allTransactions == null || !allTransactions.Any())
                return Enumerable.Empty<ReportingTransactionDto>();

            return allTransactions
                .Skip(currentPage * pageSize)
                .Take(pageSize);
        }

        public string GetPageInfo()
        {
            if (totalCount == 0)
            {
                return "0-0 of 0";
            }

            var start = (currentPage * pageSize) + 1;
            var end = Math.Min((currentPage + 1) * pageSize, totalCount);
            return $"{start}-{end} of {totalCount}";
        }

        public void OnPageSizeChange(object value)
        {
            pageSize = (int)value;
            currentPage = 0;
            StateHasChanged();
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                StateHasChanged();
            }
        }

        public void NextPage()
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                StateHasChanged();
            }
        }

        public void OnEdit()
        {
        }

        public void OnDownload()
        {
        }

        public void ApplyFilters()
        {
            LoadData();
            StateHasChanged();
        }

        protected async Task LoadTransactionsAsync()
        {
            var filter = CurrentFilters;

            var result = await Querier.Send(
                new GetReportingTransactionsQuery(
                    filter.CustomerSiteId,
                    filter.WasteStreamId,
                    filter.ContainerTypeId,
                    filter.StartDate,
                    filter.EndDate
                )
            );

            if (result.Status == BluQube.Constants.QueryResultStatus.Succeeded)
            {
                TabTransactions[selectedIndex] = result.Data.Transactions.ToList();

                BuildAllCharts();
                LoadData(); 
                StateHasChanged();
            }
        }

        protected async Task OnTabChanged(int index)
        {
            selectedIndex = index;

            showCustomerSiteDropdown = false;
            showDateRangeDropdown = false;
            showWasteStreamDropdown = false;
            showContainerTypeDropdown = false;

            await LoadTransactionsAsync();
        }

        protected void BuildAllCharts()
        {
            BuildMonthlyChart();
            BuildTransactionCountChart();
            BuildTotalValueChart();
            BuildPerformanceDonut();

            if (selectedIndex == 0)
                BuildStreamPerformanceChart();

            if (selectedIndex == 1)
            {
                BuildTotalRevenueValueChart();
                BuildRevenueCharts();
            }
        }

        protected void BuildMonthlyChart()
        {
            var data = GetFilteredTransactions();

            if (!data.Any())
            {
                monthlyChartData = new();
                return;
            }

            if (IsShortRange())
            {
                monthlyChartData = data
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key.Day,
                        Month = g.Key.ToString("dd MMM"),
                        Value = g.Sum(x => x.NetWeight)
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
            else
            {

                monthlyChartData = data
                    .GroupBy(t => t.TransactionDate.Month)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key,
                        Month = new DateTime(g.First().TransactionDate.Year, g.Key, 1).ToString("MMM"),
                        Value = g.Sum(x => x.NetWeight)
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
        }

        protected void BuildTransactionCountChart()
        {
            var data = GetFilteredTransactions();
            if (!data.Any())
            {
                transactionCountChartData = new();
                return;
            }

            if (IsShortRange())
            {
                transactionCountChartData = data
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key.Day,
                        Month = g.Key.ToString("dd MMM"),
                        Value = g.Count()
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
            else
            {
                transactionCountChartData = data
                    .GroupBy(t => t.TransactionDate.Month)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key,
                        Month = new DateTime(g.First().TransactionDate.Year, g.Key, 1).ToString("MMM"),
                        Value = g.Count()
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
        }

        protected void BuildTotalValueChart()
        {
            var data = GetFilteredTransactions();
            if (!data.Any())
            {
                totalValueChartData = new();
                return;
            }

            if (IsShortRange())
            {
                totalValueChartData = data
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key.Day,
                        Month = g.Key.ToString("dd MMM"),
                        Value = g.Sum(x => x.TonnageTransaction)
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
            else
            {
                totalValueChartData = data
                    .GroupBy(t => t.TransactionDate.Month)
                    .Select(g => new MonthlyMetricDto
                    {
                        MonthNumber = g.Key,
                        Month = new DateTime(g.First().TransactionDate.Year, g.Key, 1).ToString("MMM"),
                        Value = g.Sum(x => x.TonnageTransaction)
                    })
                    .OrderBy(x => x.MonthNumber)
                    .ToList();
            }
            if (totalValueChartData.Count == 1)
            {
                var point = totalValueChartData[0];
                totalValueChartData.Add(new MonthlyMetricDto
                {
                    MonthNumber = point.MonthNumber + 1,
                    Month = point.Month + " ",
                    Value = point.Value,
                });
            }
        }

        protected Dictionary<int, List<MonthlyMetricDto>> WasteProcessingMultiYearData { get; set; } = new();

        protected void BuildRevenueCharts()
        {
            var today = DateTime.Today;
            var currentYear = today.Year;
            var lastYear = currentYear - 1;

            var filter = CurrentFilters;

            var filteredTransactions = GetFilteredTransactions();

            BuildTotalRevenueValueChart();
            BuildMonthlyAverageCostChart(filteredTransactions, currentYear);
            BuildTonnageOfListedTransactions(filteredTransactions, currentYear);
            BuildTransactionCountRevenueChart(filteredTransactions, currentYear);
            BuildLastYearToDateChart(filteredTransactions);
            BuildLastFullYearChart();
            BuildWasteProcessingMultiYearChart(filteredTransactions);
        }

        protected void BuildTonnageOfListedTransactions(List<ReportingTransactionDto> data, int year)
        {
            tonnageRevenueData = data
                .Where(t => t.TransactionDate.Year == year)
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new MonthlyMetricDto
                {
                    MonthNumber = g.Key,
                    Month = new DateTime(year, g.Key, 1).ToString("MMM"),
                    Value = g.Sum(x => x.NetWeight)
                })
                .OrderBy(x => x.MonthNumber)
                .ToList();
        }

        protected void BuildMonthlyAverageCostChart(List<ReportingTransactionDto> data, int year)
        {
            var start = CurrentFilters.StartDate ?? new DateTime(year, 1, 1);
            var end = CurrentFilters.EndDate ?? new DateTime(year, 12, 31);

            monthlyAverageCostData = data
                .Where(t => t.TransactionDate.Date >= start.Date &&
                            t.TransactionDate.Date <= end.Date)
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new MonthlyMetricDto
                {
                    MonthNumber = g.Key,
                    Month = new DateTime(g.First().TransactionDate.Year, g.Key, 1).ToString("MMM"),
                    Value = g.Average(x => x.TransactionValue)
                })
                .OrderBy(x => x.MonthNumber)
                .ToList();
        }

        protected void BuildTransactionCountRevenueChart(List<ReportingTransactionDto> data, int year)
        {
            var start = CurrentFilters.StartDate ?? new DateTime(year, 1, 1);
            var end = CurrentFilters.EndDate ?? new DateTime(year, 12, 31);

            transactionCountChartData = data
                .Where(t => t.TransactionDate.Date >= start.Date &&
                            t.TransactionDate.Date <= end.Date)
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new MonthlyMetricDto
                {
                    MonthNumber = g.Key,
                    Month = new DateTime(g.First().TransactionDate.Year, g.Key, 1).ToString("MMM"),
                    Value = g.Count()
                })
                .OrderBy(x => x.MonthNumber)
                .ToList();
        }

        protected void BuildLastYearToDateChart(List<ReportingTransactionDto> data)
        {
            var today = DateTime.Today;
            var currentYear = today.Year;
            var lastYear = currentYear - 1;

            lastYearToDateData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    decimal value = 0;

                    value += data
                        .Where(t =>
                            t.TransactionDate.Year == lastYear &&
                            t.TransactionDate.Month == month)
                        .Sum(t => t.TransactionValue);
                    if (month <= today.Month)
                    {
                        value += data
                            .Where(t =>
                                t.TransactionDate.Year == currentYear &&
                                t.TransactionDate.Month == month &&
                                t.TransactionDate.Date <= today)
                            .Sum(t => t.TransactionValue);
                    }

                    return new MonthlyMetricDto
                    {
                        MonthNumber = month,
                        Month = new DateTime(lastYear, month, 1).ToString("MMM"),
                        Value = value
                    };
                })
                .ToList();
        }

        protected void BuildLastFullYearChart()
        {

            var year = DateTime.Today.Year - 1;

            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);

            var data = GetFilteredTransactions()
                .Where(t => t.TransactionDate.Date >= start && t.TransactionDate.Date <= end)
                .ToList();
            lastFullYearData = data
                .GroupBy(t => t.TransactionDate.Month)
                .Select(g => new MonthlyMetricDto
                {
                    MonthNumber = g.Key,
                    Month = new DateTime(year, g.Key, 1).ToString("MMM"),
                    Value = g.Sum(x => x.TransactionValue)
                })
                .OrderBy(x => x.MonthNumber)
                .ToList();
        }

        protected void BuildWasteProcessingMultiYearChart(List<ReportingTransactionDto> data)
        {
            var years = new[] { DateTime.Today.Year - 2, DateTime.Today.Year - 1, DateTime.Today.Year };

            WasteProcessingMultiYearData = years.ToDictionary(
                year => year,
                year => Enumerable.Range(1, 12)
                    .Select(m => new MonthlyMetricDto
                    {
                        MonthNumber = m,
                        Month = new DateTime(year, m, 1).ToString("MMM"),
                        Value = data
                            .Where(t => t.TransactionDate.Date >= new DateTime(year, 1, 1) &&
                                        t.TransactionDate.Date <= new DateTime(year, 12, 31) &&
                                        t.TransactionDate.Month == m)
                            .Sum(t => t.TransactionValue)
                    }).ToList()
            );
        }

        protected void BuildStreamPerformanceChart()
        {
            streamPerformanceData = GetFilteredTransactions()
                .GroupBy(t => t.WasteStreamName)

                  .Select(g => new StreamPerformanceDto
                  {
                      StreamName = g.Key,
                      Value = g.Sum(x => x.NetWeight)
                  })
                .OrderByDescending(x => x.Value)
                .Take(3)
                .ToList();
        }
        public string GetMobileButtonClass(string path)
        {
            return IsActive(path)
                ? "menu-btn menu-btn-active w-100 text-start"
                : "menu-btn w-100 text-start";
        }

        public string currentView = "reports-analytics";

        public void Navigate(string view)
        {
            mobileMenuOpen = false;
            currentView = view;
        }

        public bool IsActive(string view)
        {
            return currentView.Equals(view, StringComparison.OrdinalIgnoreCase);
        }


        public string GetButtonClass(string path)
        {
            return IsActive(path)
                ? "menu-btn menu-btn-active"
                : "menu-btn";
        }

        public bool mobileMenuOpen = false;

        public void ToggleMobileMenu()
        {
            mobileMenuOpen = !mobileMenuOpen;
            StateHasChanged();
        }

    }
}
