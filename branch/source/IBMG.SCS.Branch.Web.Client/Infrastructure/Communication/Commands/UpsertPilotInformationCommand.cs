using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;
using System;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands
{
    [BluQubeCommand(Path = "commands/pilot/upsert")]
    public record UpsertPilotInformationCommand(
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
        byte[]? ProfileImageBytes,
        string? ProfileImageFileName,
        string? ProfileImageContentType,
        string CreatedBy,
        DateTime CreatedOn,
        string? ModifiedBy,
        DateTime? ModifiedOn
    ) : ICommand<UpsertPilotInformationCommandResult>;
}
