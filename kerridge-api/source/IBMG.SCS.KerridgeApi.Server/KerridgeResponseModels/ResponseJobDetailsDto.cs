namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseJobDetailsDto
    {
        public KerridgeJobDetailsResponseBody Response { get; set; }
    }

    public class KerridgeJobDetailsResponseBody
    {
        public List<KerridgeJobDetailsResultItem> Results { get; set; }
    }

    public class KerridgeJobDetailsResultItem
    {
        public string Jobdesc_1 { get; set; }

        public string Name_2 { get; set; }

        public int Active_0 { get; set; }

        public decimal Spend_1 { get; set; }
    }
}