namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class ReportFilterState
    {
        public int? CustomerSiteId { get; set; }
        public int? WasteStreamId { get; set; }
        public int? ContainerTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
