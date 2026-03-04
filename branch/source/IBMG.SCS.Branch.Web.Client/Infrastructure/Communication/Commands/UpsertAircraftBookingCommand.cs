using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands
{
    [BluQubeCommand(Path = "commands/aircraft-booking/upsert")]
    public record UpsertAircraftBookingCommand(
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
    ) : ICommand<UpsertAircraftBookingCommandResult>;
}
