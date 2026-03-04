using System;
using System.Collections.Generic;
using System.Text;

namespace IBMG.SCS.Infrastructure.Entities
{
    public class ReportingTransaction
    {
        public int Id { get; set; }
        public long TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public int CustomerSiteId { get; set; }
        public int WasteStreamId { get; set; }
        public int ContainerTypeId { get; set; }

        public decimal Tonnage { get; set; }
        public decimal TransactionValue { get; set; }
        public decimal TonnageTransaction {  get; set; }
        public string OrderNumber { get; set; }
        public string TicketNumber { get; set; }
        public string ActivityStatus { get; set; }
        public string CollectionType { get; set; }

        public decimal NetWeight { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
