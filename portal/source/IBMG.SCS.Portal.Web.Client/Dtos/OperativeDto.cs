// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class OperativeDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid JobRole { get; set; }

        public string OperativeNumber { get; set; }

        public decimal TnxLimit { get; set; }

        public decimal DailyLimit { get; set; }

        public decimal WeeklyLimit { get; set; }

        public decimal MonthlyLimit { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Status Status { get; set; }

        public Guid TradeCardId { get; set; } = Guid.Empty;

        public string TradeCardNumber { get; set; }

        public decimal? OverrideTnxLimit { get; set; }

        public decimal? OverrideDailyLimit { get; set; }

        public decimal? OverrideWeeklyLimit { get; set; }

        public decimal? OverrideMonthlyLimit { get; set; }

        public DateTime? OverrideEndDate { get; set; }
    }
}