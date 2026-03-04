// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class OperativeTimesheet
    {
        public string? Operative { get; set; }

        public decimal? TotalSpent { get; set; }

        public Dictionary<DateTime, decimal> DailyValues { get; set; } = new();

        public string Branch { get; set; }

        public string TransactionType { get; set; }

        public string Category { get; set; }
    }
}