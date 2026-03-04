namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseCustomerListDto
    {
        public KerridgeCustomerResponseBody Response { get; set; }
    }

    public class KerridgeCustomerResponseBody
    {
        public List<KerridgeCustomerItem> Results { get; set; }
    }

    public class KerridgeCustomerItem
    {
        public string Account_1 { get; set; }

        public string Name_1 { get; set; }

        public int Active_0 { get; set; }
    }
}