namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class FaultyGoodsKpiDto
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Value { get; set; }

        public double Target { get; set; }

        public int TotalOrders { get; set; }

        public int FaultyOrders { get; set; }

        public decimal FaultyPercentage { get; set; }
    }
}