namespace IBMG.SCS.KerridgeApi.Server.KerridgeDwhModels
{
    public class ScsSalesAndOrders
    {
        public int System_ID { get; set; }

        public string Order_Type { get; set; } = default!;

        public int Branch_ID { get; set; }

        public int Branch_Code { get; set; }

        public string? Branch_Desc { get; set; }

        public int? Operative_ID { get; set; }

        public string? Operative_Code { get; set; }

        public string? Operative_Desc { get; set; }

        public int Customer_ID { get; set; }

        public int Customer_Code { get; set; }

        public string Customer_Desc { get; set; } = default!;

        public string? Order_Category { get; set; }

        public string? Product_Category { get; set; }

        public string? Product_Category_Desc { get; set; }

        public DateOnly? Transaction_Date { get; set; }

        public decimal? Invoiced_Sales_Total { get; set; }

        public decimal? Open_Orders_Total { get; set; }
    }

}