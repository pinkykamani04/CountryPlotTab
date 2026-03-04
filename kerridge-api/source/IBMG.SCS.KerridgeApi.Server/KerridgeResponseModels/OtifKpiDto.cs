namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class OtifKpiDto
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Value { get; set; }

        public double Target { get; set; }

        public decimal OnTimePercentage { get; set; }

        public decimal InFullPercentage { get; set; }
    }
}