// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.Models
{
    public class Operatives
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Guid JobRole { get; set; }

        public string OperativeNumber { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsRowDeleted { get; set; }

        public Guid TradeCardId { get; set; }

        public decimal DailyLimit { get; set; }

        public decimal MonthlyLimit { get; set; }

        public decimal? OverrideDailyLimit { get; set; }

        public DateTime? OverrideEndDate { get; set; }

        public decimal? OverrideMonthlyLimit { get; set; }

        public decimal? OverrideTnxLimit { get; set; }

        public decimal? OverrideWeeklyLimit { get; set; }

        public decimal TnxLimit { get; set; }

        public decimal WeeklyLimit { get; set; }
    }
}