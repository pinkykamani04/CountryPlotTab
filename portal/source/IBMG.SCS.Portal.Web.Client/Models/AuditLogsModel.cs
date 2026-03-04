// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class AuditLogsModel
    {
        public Guid Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public string User { get; set; }

        public string Category { get; set; }

        public string Action { get; set; }
    }
}