namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class FlightInfoModel
    {
        public Guid Id { get; set; }

        public string DepartureCode { get; set; }

        public string DepartureTime { get; set; }

        public string ArrivalCode { get; set; }

        public string ArrivalTime { get; set; }

        public string Date { get; set; }

        public string Route { get; set; }

        public string FullFromLocation { get; set; }

        public string FullToLocation { get; set; }

        public DateTime FromDateTime { get; set; }

        public DateTime ToDateTime { get; set; }

        public string AircraftType { get; set; }

        public Guid PilotId { get; set; }
    }
}
