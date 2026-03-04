namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class GroupedOrdersDto
    {
        public string GroupKey { get; set; }

        public List<OrderDto> Items { get; set; } = new();
    }
}