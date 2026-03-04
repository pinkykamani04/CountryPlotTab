namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseOperativeValidationDto
    {
        public KerridgeValidateOperativeResponseBody Response { get; set; }
    }

    public class KerridgeValidateOperativeResponseBody
    {
        public List<KerridgeOperativeValidationResultItem> Results { get; set; }
    }

    public class KerridgeOperativeValidationResultItem
    {
        public string Firstname_1 { get; set; }

        public string Surname_1 { get; set; }

        public int Activestatus_1 { get; set; }
    }
}