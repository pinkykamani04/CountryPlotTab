// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class TradeValidationCardDto
    {
        public string OperativeName { get; set; } = "-";

        public string OperativeId { get; set; } = "-";

        public string JobNumber { get; set; } = "-";

        public string JobAddress { get; set; } = "-";

        public decimal TxnLimit { get; set; }

        public decimal DailyLimit { get; set; }

        public decimal WeeklyLimit { get; set; }

        public decimal MonthlyLimit { get; set; }

        public string CardNumber { get; set; } = "-";

        public double SpendRemaining { get; set; }

        public string Status { get; set; }
    }
}