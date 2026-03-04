// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class ValidationDto
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int ValidationType { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsMandatory { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}