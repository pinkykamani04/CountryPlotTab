using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands
{
    [BluQubeCommand(Path = "commands/aircraft/upsert")]
    public record UpsertAircraftCommand(
         Guid Id,
         Guid AircraftId, 
         string TailNumber,
         string Location,
         string Model,
         long YearOfManufacture,
         string Manufacturer,
         string AircraftType,
         string VariantType,
         string TypeRating,
         bool IsRowDeleted,
         string CreatedBy,
         DateTime CreatedOn,
         string? ModifiedBy,
         DateTime? ModifiedOn
     ) : ICommand<UpsertAircraftCommandResult>;
}
