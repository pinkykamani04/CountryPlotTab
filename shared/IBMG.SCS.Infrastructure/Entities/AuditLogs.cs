// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class AuditLogs
    {
        public Guid Id { get; set; }

        public DateOnly CreatedOnDate { get; set; }

        public TimeOnly CreatedOnTime { get; set; }

        public string User { get; set; }

        public string Category { get; set; }

        public string Action { get; set; }
    }
}