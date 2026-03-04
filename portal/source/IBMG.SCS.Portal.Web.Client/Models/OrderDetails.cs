// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class OrderDetails
    {
        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string? Operative { get; set; }

        public int OrderNo { get; set; }

        public string? Branch { get; set; }

        public decimal Total { get; set; }

        public string? TransactionType { get; set; }

        public string? Category { get; set; }
    }
}