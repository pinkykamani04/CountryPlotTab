namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class OperativeSpendDto
    {
        public Guid CustomerId { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public decimal TotalSpend { get; set; }

        public string Currency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Notes { get; set; }
    }
}