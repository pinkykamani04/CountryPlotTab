// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeDwhModels
{
    public class ScsCustomer
    {
        public int Customer_ID { get; set; }

        public string? Customer_Code { get; set; }

        public string? Customer_Desc { get; set; }

        public string? Credit_Status { get; set; }

        public long Credit_Limit { get; set; }

        public decimal Credit_Available { get; set; }
    }
}