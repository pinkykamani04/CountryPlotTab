namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseDaySpendDto
    {
        public KerridgeDaySpendResponseBody Response { get; set; }
    }

    public class KerridgeDaySpendResponseBody
    {
        public KerridgeTotals Grandtotals { get; set; }
    }

    public class KerridgeTotals
    {
        public decimal Goods_2 { get; set; }
    }
}