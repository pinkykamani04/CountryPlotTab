// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeDwhModels
{
    public class ScsSalesAndOrdersLines
    {
        public int System_id { get; set; }

        public string? Order_Status { get; set; }

        public int? Operative_ID { get; set; }

        public string? Operative_Code { get; set; }

        public string? Operative_Desc { get; set; }

        public int? Customer_ID { get; set; }

        public string? Customer_Code { get; set; }

        public string? Customer_Desc { get; set; }

        public int Branch_ID { get; set; }

        public int Branch_Code { get; set; }

        public string? Branch_Desc { get; set; }

        public int Order_Number { get; set; }

        public string Order_Number_Unique { get; set; } = default!;

        public int Order_Line { get; set; }

        public string Order_Type { get; set; } = default!;

        public string? Order_Category { get; set; }

        public DateOnly? Transaction_Date { get; set; }

        public string? Product_ID { get; set; }

        public string? Product_Code { get; set; }

        public string? Product_Desc { get; set; }

        public string? Level1_Code { get; set; }

        public string? Level1_Desc { get; set; }

        public int? Reason_ID { get; set; }

        public string? Reason_Desc { get; set; }

        public decimal? Invoiced_Sales_Total { get; set; }
    }
}