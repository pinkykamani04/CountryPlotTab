using BluQube.Queries;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public class GetAllPilotInformationQueryResult(IReadOnlyList<GetAllPilotInformationQueryResult.PilotInformationItemDto> Pilots
    ) : IQueryResult
    {
        public record PilotInformationItemDto(
            Guid Id,
            string FullName,
            DateTime DateOfBirth,
            long ContactNumber,
            string EmailAddress,
            string ContactAddress,
            string Location,
            string LicenseNumber,
            string LicenseType,
            DateTime LicenseInsuranceDate,
            DateTime LicenseExpiryDate,
            DateTime MedicalCertificate,
            string ProfileImageName,
     
            string CreatedBy,
            DateTime CreatedOn,
            string? ModifiedBy,
            DateTime? ModifiedOn
        );
    }
}
