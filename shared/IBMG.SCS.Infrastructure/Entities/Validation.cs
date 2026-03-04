// Copyright (c) IBMG. All rights reserved.

using PearDrop.Domain;
using PearDrop.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class Validation : Entity, IAggregateRoot
    {
        public Guid Id {  get; set; }
        public int UserId { get; set; }

        public int ValidationType { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsMandatory { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
