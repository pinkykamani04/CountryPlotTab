namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class InvoiceAccuracyKpiDto
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Value { get; set; }

        public double Target { get; set; }

        public int TotalInvoices { get; set; }

        public int CorrectInvoices { get; set; }

        public decimal AccuracyPercentage { get; set; }
    }
}