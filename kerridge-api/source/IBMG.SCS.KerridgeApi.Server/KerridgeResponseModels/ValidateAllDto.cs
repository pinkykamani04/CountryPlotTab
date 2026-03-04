namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ValidateAllDto
    {
        public string AccountCode { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string TradeCardNumber { get; set; }

        public string OperativeId { get; set; }

        public bool OperativeTradeStatus { get; set; }

        public string JobReference { get; set; }

        public string JobDescription { get; set; }

        public string JobReferenceStatus { get; set; }
    }
}