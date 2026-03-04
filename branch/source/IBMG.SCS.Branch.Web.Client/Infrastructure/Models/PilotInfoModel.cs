namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class PilotInfoModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public long ContactNumber { get; set; }

        public string EmailAddress { get; set; } = string.Empty;

        public string ContactAddress { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public string LicenseType { get; set; } = string.Empty;

        public DateTime LicenseInsuranceDate { get; set; }

        public DateTime LicenseExpiryDate { get; set; }

        public DateTime MedicalCertificate { get; set; }

        public string ProfileImageName { get; set; } = string.Empty;


        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

    }
}