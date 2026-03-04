using BluQube.Queries;
using System;
using System.Collections.Generic;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public record GetPilotInformationQueryResult(
        IReadOnlyList<GetPilotInformationQueryResult.PilotInformationItemDto> Pilots
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
