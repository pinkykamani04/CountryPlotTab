namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class JobDetailsDto
    {
        public string JobDescription { get; set; }

        public string CustomerName { get; set; }

        public bool Active { get; set; }

        public decimal Spend { get; set; }
    }
}