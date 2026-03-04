using BluQube.Queries;
using System;
using System.Collections.Generic;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public record GetAllAircraftBookingQueryResult(
          IReadOnlyList<GetAllAircraftBookingQueryResult.AircraftBookingItemDto> Bookings
      ) : IQueryResult
    {
        public record AircraftBookingItemDto(
            Guid Id,
            Guid AircraftId,
            Guid PilotId,
            string TailNumber,
            DateTime FromDate,
            TimeSpan FromTime,
            DateTime ToDate,
            TimeSpan ToTime,
            string FromLocation,
            string ToLocation,
            string Status,
            bool IsRowDeleted,
            string CreatedBy,
            DateTime CreatedOn,
            string? ModifiedBy,
            DateTime? ModifiedOn
        );
    }
}
