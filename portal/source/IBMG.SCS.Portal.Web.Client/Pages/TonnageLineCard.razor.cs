using IBMG.SCS.Portal.Web.Client.Dtos;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Pages
{
    public partial class TonnageLineCard : ComponentBase
    {
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public IEnumerable<MonthlyMetricDto>? Data { get; set; }

        private IEnumerable<MonthlyMetricDto> FormattedData =>
            Data?.OrderBy(d => d.MonthNumber) ?? Enumerable.Empty<MonthlyMetricDto>();

        private List<GroupedMetricDto> GetAggregatedGroups(int size)
        {
            var ordered = FormattedData.ToList();
            var result = new List<GroupedMetricDto>();
            for (int i = 0; i < ordered.Count; i += size)
            {
                var slice = ordered.Skip(i).Take(size).ToList();
                var first = slice.First().Month.Length >= 3 ? slice.First().Month[..3] : slice.First().Month;
                var last = slice.Last().Month.Length >= 3 ? slice.Last().Month[..3] : slice.Last().Month;
                result.Add(new GroupedMetricDto
                {
                    Label = $"{first}-{last}",
                    Total = (double)slice.Sum(x => x.Value)
                });
            }
            return result;
        }

        private class GroupedMetricDto
        {
            public string Label { get; set; } = "";
            public double Total { get; set; }
        }
    }
}
