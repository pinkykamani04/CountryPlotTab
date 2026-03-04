using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class AircraftInfo
    {
        public Guid Id { get; set; }
        public Guid AircraftId {  get; set; }
        public string TailNumber {  get; set; }
        public string Location {  get; set; }
        public string Model {  get; set; }
        public long YearOfManufacture { get; set; }
        public string Manufacturer { get; set; }
        public string AircraftType { get; set; }
        public string VariantType { get; set; }
        public string TypeRating { get; set; }
        public bool IsRowDeleted {  get; set; }
        public string CreatedBy {  get; set; }
        public DateTime CreatedOn {  get; set; }
        public string? ModifiedBy {  get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
