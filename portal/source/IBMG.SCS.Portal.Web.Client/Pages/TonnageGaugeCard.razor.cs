using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Pages
{
    public partial class TonnageGaugeCard : ComponentBase
    {
        [Parameter] 
        public string Title { get; set; } = string.Empty;
        [Parameter] 
        public double AchievedPercentage { get; set; }
        [Parameter] 
        public decimal TotalValue { get; set; }

        public decimal GetAchievedTonnage()
        {
            return (decimal)(AchievedPercentage * (double)TotalValue / 100.0);
        }

        public decimal GetRemainingTonnage()
        {
            return TotalValue - GetAchievedTonnage();
        }

        public class DonutSlice
        {
            public string Label { get; set; } = string.Empty;

            public double Value { get; set; }
        }

        public List<DonutSlice> DonutData
        {
            get
            {
                var achieved = (double)GetAchievedTonnage();
                var remaining = (double)GetRemainingTonnage();

                if (achieved <= 0) achieved = 0.0001;
                if (remaining <= 0) remaining = 0.0001;

                return new List<DonutSlice>
                {
                    new DonutSlice { Label = "Remaining", Value = remaining },
                    new DonutSlice { Label = "Achieved", Value = achieved }
                };
            }
        }
    }
}
