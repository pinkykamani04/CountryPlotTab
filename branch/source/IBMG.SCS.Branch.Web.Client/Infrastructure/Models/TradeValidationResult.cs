// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class TradeValidationResult
    {
        public Guid CustomerId { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; } = string.Empty;

        public string JobNumber { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public string JobAddress { get; set; } = "Active";

        public decimal TxnLimit { get; set; }

        public decimal DailyLimit { get; set; }

        public decimal WeeklyLimit { get; set; }

        public decimal MonthlyLimit { get; set; }

        public decimal SpendRemaining { get; set; }
    }
}