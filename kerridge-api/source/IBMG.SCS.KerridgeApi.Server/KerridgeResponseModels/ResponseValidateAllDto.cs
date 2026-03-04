namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseValidateAllDto
    {
        public KerridgeValidateResponseBody Response { get; set; }
    }

    public class KerridgeValidateResponseBody
    {
        public List<KerridgeValidateResultItem> Results { get; set; }
    }

    public class KerridgeValidateResultItem
    {
        public string Account_1 { get; set; }

        public string Firstname_1 { get; set; }

        public string Surname_1 { get; set; }

        public string Tradecard_1 { get; set; }

        public string Operativeid_1 { get; set; }

        public int Activestatus_1 { get; set; }

        public string Jobref_2 { get; set; }

        public string Jobdesc_2 { get; set; }

        public string Active_2 { get; set; }
    }
}