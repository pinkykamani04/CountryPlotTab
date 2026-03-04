using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class FlightList
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public List<FlightInfoModel> Flights { get; set; } = new();

        [Parameter]
        public string CardClass { get; set; } = string.Empty;

        [Parameter]
        public string ActionButtonText { get; set; } = "Details";

        [Parameter]
        public string NoFlightsText { get; set; } = "No flights";

        [Parameter]
        public int DepartureColSize { get; set; } = 4;

        [Parameter]
        public int MiddleColSize { get; set; } = 4;

        [Parameter]
        public int ArrivalColSize { get; set; } = 4;
    }
}
