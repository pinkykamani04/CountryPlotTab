namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseTradeCardValidationDto
    {
        public KerridgeValidateTradeCardResponseBody Response { get; set; }
    }

    public class KerridgeValidateTradeCardResponseBody
    {
        public List<KerridgeTradeCardValidationResultItem> Results { get; set; }
    }

    public class KerridgeTradeCardValidationResultItem
    {
        public string Firstname_1 { get; set; }

        public string Surname_1 { get; set; }

        public int Activestatus_1 { get; set; }
    }
}