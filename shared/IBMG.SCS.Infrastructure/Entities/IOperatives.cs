// Copyright (c) IBMG. All rights reserved.

using PearDrop.Domain.Contracts;

namespace IBMG.SCS.Infrastructure.Entities
{
    public interface IOperatives : IEntity, IAggregateRoot
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobRole { get; set; }

        public string OperativeNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Status Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsRowDeleted { get; set; }
    }
}