namespace IBMG.SCS.KerridgeApi.Server.Models
{
    public class SpendLimits
    {
        public decimal DailyLimit { get; set; }

        public decimal MonthlyLimit { get; set; }

        public decimal TnxLimit { get; set; }

        public decimal WeeklyLimit { get; set; }

        public decimal? OverrideDailyLimit { get; set; }

        public decimal? OverrideMonthlyLimit { get; set; }

        public decimal? OverrideTnxLimit { get; set; }

        public decimal? OverrideWeeklyLimit { get; set; }
    }
}