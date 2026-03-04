namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseSpenLimitDto
    {
        public KerridgeSpendLimitResponseBody Response { get; set; }
    }

    public class KerridgeSpendLimitResponseBody
    {
        public KerridgeGrandTotals Grandtotals { get; set; }

        public List<KerridgeSpendLimitResultItem> Results { get; set; }
    }

    public class KerridgeGrandTotals
    {
        public decimal Spendlim_0 { get; set; }
    }

    public class KerridgeSpendLimitResultItem
    {
        public string Custacc_0 { get; set; }

        public decimal Spendlim_0 { get; set; }
    }
}