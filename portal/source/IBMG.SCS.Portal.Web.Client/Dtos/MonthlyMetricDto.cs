namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class MonthlyMetricDto
    {
        public int MonthNumber { get; set; }
        public string Month { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
