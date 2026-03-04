// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsRowDeleted { get; set; } = false;
    }
}