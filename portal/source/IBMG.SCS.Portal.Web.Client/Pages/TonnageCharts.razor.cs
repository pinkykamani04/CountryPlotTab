using IBMG.SCS.Portal.Web.Client.Dtos;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Pages
{
    public partial class TonnageCharts : ComponentBase
    {
        [Parameter] public List<MonthlyMetricDto> MonthlyChartData { get; set; } = new();
        [Parameter] public List<MonthlyMetricDto> TransactionCountChartData { get; set; } = new();
        [Parameter] public List<MonthlyMetricDto> TotalValueChartData { get; set; } = new();
        [Parameter] public List<StreamPerformanceDto> StreamPerformanceData { get; set; } = new();

        [Parameter] public double AchievedPercentage { get; set; }
        [Parameter] public double RemainingPercentage { get; set; }
        [Parameter] public decimal TotalStreamValue { get; set; }

        protected IEnumerable<PerformanceSlice> PerformanceData =>
            new List<PerformanceSlice>
            {
                new() { Label = "Achieved", Value = AchievedPercentage },
                new() { Label = "Remaining", Value = RemainingPercentage }
            };

        public class PerformanceSlice
        {
            public string Label { get; set; } = string.Empty;
            public double Value { get; set; }
        }

        protected decimal GetAchievedTonnage()
        {
            return (decimal)(AchievedPercentage * (double)TotalStreamValue / 100);
        }

        protected string GetArcEndPoint()
        {
            double angle = -135 + (270 * AchievedPercentage / 100);
            double radians = angle * Math.PI / 180;

            double x = 100 + 70 * Math.Cos(radians);
            double y = 120 + 70 * Math.Sin(radians);

            return $"{x:F2} {y:F2}";
        }
    }
}

