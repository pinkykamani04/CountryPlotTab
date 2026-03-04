// Copyright (c) IBMG. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace IBMG.SCS.KerridgeApi.Server.Models
{
    public class TradeCards
    {
        public Guid Id { get; set; }

        public string TradeCardNumber { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsRowDeleted { get; set; }

        public Guid? AssigneeId { get; set; }
    }
}