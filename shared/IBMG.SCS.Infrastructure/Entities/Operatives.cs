// Copyright (c) IBMG. All rights reserved.

using PearDrop.Domain;
using PearDrop.Domain.Contracts;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class Operatives : Entity, IAggregateRoot
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

        public decimal? OverrideTnxLimit { get; set; }

        public decimal? OverrideDailyLimit { get; set; }

        public decimal? OverrideWeeklyLimit { get; set; }

        public decimal? OverrideMonthlyLimit { get; set; }

        public DateTime? OverrideEndDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsRowDeleted { get; set; } = false;

        public Guid TradeCardId { get; set; }
    }
}