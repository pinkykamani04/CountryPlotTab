namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class ReportingTransactionDto
    {
        public int Id { get; set; }
        public long TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public int CustomerSiteId { get; set; }
        public string CustomerSiteName { get; set; } = string.Empty;

        public int WasteStreamId { get; set; }
        public string WasteStreamName { get; set; } = string.Empty;

        public int ContainerTypeId { get; set; }
        public string ContainerTypeName { get; set; } = string.Empty;

        public decimal Tonnage { get; set; }
        public decimal NetWeight { get; set; }
        public decimal TransactionValue { get; set; }
        public decimal TonnageTransaction {  get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;

        public string ActivityStatus { get; set; } = string.Empty;
        public string CollectionType { get; set; } = string.Empty;
    }
}
