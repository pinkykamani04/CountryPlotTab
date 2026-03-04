// Copyright (c) IBMG. All rights reserved.

using PearDrop.Domain.Contracts;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class TradeCards : BaseEntity, IAggregateRoot
    {
        public Guid Id { get; set; }

        public string TradeCardNumber { get; set; }

        public Guid? AssigneeId { get; set; }

        public string Status { get; set; }
    }
}