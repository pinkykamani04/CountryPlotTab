namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ProductsBySpend
    {
        public string? ProductCategory { get; set; }

        public string ProductName { get; set; }

        public decimal? ProductSpend { get; set; }
    }
}