namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class CustomerBySpendDto
    {
        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public decimal CustmomerSpend { get; set; }
    }
}