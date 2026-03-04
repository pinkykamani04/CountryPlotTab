// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class DisplayRow
    {
        public string Operative { get; set; }

        public decimal TotalSpent { get; set; }

        public Dictionary<DateTime, decimal> Columns { get; set; } = new();

        public string TransactionType { get; set; }

        public string Category { get; set; }

        public string Branch { get; set; }

        public string OrderNo { get; set; }
    }
}