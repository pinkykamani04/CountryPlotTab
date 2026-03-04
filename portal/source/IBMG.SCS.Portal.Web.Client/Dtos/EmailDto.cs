// Copyright (c) IBMG. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class EmailDto
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string SupportEmail { get; set; } = string.Empty;
    }
}