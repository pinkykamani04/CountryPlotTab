using BluQube.Commands;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults
{
    public record UpsertAircraftCommandResult(
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
  ) : ICommandResult;
}
