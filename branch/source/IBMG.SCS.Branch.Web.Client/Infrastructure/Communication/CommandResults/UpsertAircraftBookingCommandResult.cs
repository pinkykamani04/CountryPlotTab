using BluQube.Commands;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults
{
    public record UpsertAircraftBookingCommandResult(
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
     ) : ICommandResult;
}
