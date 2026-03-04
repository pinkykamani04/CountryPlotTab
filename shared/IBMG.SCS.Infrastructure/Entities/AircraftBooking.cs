using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class AircraftBooking
    {
        public Guid Id { get; set; }
        public Guid AircraftId { get; set; }
        public Guid PilotId { get; set; }
        public string TailNumber { get; set; }
        public DateTime FromDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public DateTime ToDate { get; set; } 
        public TimeSpan ToTime { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Status { get; set; }
        public bool IsRowDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
