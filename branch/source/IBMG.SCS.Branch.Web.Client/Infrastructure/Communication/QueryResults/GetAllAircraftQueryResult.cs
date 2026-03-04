using BluQube.Queries;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public record GetAllAircraftQueryResult(IReadOnlyList<GetAllAircraftQueryResult.AircraftItemDto> Aircrafts) : IQueryResult
    {
        public record AircraftItemDto(
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
        );
    }
}
