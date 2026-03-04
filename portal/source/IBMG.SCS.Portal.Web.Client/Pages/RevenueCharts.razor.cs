using IBMG.SCS.Portal.Web.Client.Dtos;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Pages
{
    public partial class RevenueCharts : ComponentBase
    {
        [Parameter] public List<MonthlyMetricDto> TotalValueChartData { get; set; } = [];
        [Parameter] public List<MonthlyMetricDto> MonthlyAverageCostData { get; set; } = [];
        [Parameter] public List<MonthlyMetricDto> TonnageRevenueData { get; set; } = [];
        [Parameter] public List<MonthlyMetricDto> TransactionCountChartData { get; set; } = [];
        [Parameter] public List<MonthlyMetricDto> LastYearToDateData { get; set; } = [];
        [Parameter] public List<MonthlyMetricDto> LastFullYearData { get; set; } = [];
        [Parameter] public List<YearlyComparisonDto> WasteProcessingCostData { get; set; } = [];
        [Parameter] public Dictionary<int, List<MonthlyMetricDto>> WasteProcessingMultiYearData { get; set; } = new();

    }
}
