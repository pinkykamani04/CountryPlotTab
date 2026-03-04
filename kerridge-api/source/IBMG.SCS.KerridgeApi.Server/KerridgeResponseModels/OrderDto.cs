namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public double Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string OperativeId { get; set; } = string.Empty;

        public string OperativeName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        public string TransactionType { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}