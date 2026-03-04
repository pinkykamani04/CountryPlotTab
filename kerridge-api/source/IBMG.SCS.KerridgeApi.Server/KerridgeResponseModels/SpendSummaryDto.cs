namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class SpendSummaryDto
    {
        public Guid CustomerId { get; set; }

        public double TotalSpend { get; set; }

        public double CreditLimit { get; set; }

        public double AvailableCredit { get; set; }

        public int TotalSales { get; set; }
    }
}