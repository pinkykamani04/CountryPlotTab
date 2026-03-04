using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class PilotInformation
    {
        public Guid Id {  get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ContactAddress { get; set; }
        public string Location { get; set; }

        public string LicenseNumber { get; set; }
        public string LicenseType { get; set; }
        public DateTime LicenseInsuranceDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public DateTime MedicalCertificate { get; set; }

        public string ProfileImageName { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy {  get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
