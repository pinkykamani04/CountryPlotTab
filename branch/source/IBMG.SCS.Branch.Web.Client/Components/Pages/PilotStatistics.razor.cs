using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class PilotStatistics
    {
        [Parameter]
        public double TotalFlyingHours { get; set; }

        [Parameter]
        public int FlightsThisYear { get; set; }

        [Parameter]
        public int TotalFlights { get; set; }

        [Parameter]
        public int DistanceCovered { get; set; }

        [Parameter]
        public int PlanesFlown { get; set; }

        [Parameter]
        public int PicHours { get; set; }

        [Parameter]
        public int CoPilotHours { get; set; }

        [Parameter]
        public int NightHours { get; set; }

        [Parameter]
        public int InstrumentHours { get; set; }

        [Parameter]
        public int CrossCountryHours { get; set; }
    }
}