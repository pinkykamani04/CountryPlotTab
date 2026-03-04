// Copyright (c) IBMG. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class ReportIssueDto
    {
        [Required]
        public string IssueType { get; set; } = string.Empty;

        [Required]
        public string IssueDetails { get; set; } = string.Empty;
    }
}