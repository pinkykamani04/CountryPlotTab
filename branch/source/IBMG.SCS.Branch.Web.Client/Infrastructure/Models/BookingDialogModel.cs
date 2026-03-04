namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class BookingDialogModel
    {
        public Guid Id { get; set; }

        public Guid AircraftId { get; set; }

        public DateTime FromDate { get; set; }

        public TimeSpan FromTime { get; set; }

        public DateTime ToDate { get; set; }

        public TimeSpan ToTime { get; set; }

        public string FromLocation { get; set; }

        public string ToLocation { get; set; }

        public bool IsRowDeleted { get; set; }

        public Guid PilotId { get; set; }
    }
}
