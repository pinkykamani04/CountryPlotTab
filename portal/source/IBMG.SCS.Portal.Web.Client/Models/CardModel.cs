// Copyright (c) IBMG. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class CardModel
    {
        public Guid Id { get; set; }

        [Required]
        public string CardNumber { get; set; }

        public Guid? AssigneeId { get; set; }

        public string Status { get; set; } = "Active";

        public string AssigneeName { get; set; }
    }
}