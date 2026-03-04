using BluQube.Commands;
using System;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults
{
    public record UpsertPilotInformationCommandResult(
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
    ) : ICommandResult;
}
