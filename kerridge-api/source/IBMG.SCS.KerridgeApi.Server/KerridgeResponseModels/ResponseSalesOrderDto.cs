// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ResponseSalesOrderDto
    {
        public SalesOrderResponseBody? Body { get; set; }
    }

    public class SalesOrderResponseBody
    {
        public int? OrderNo { get; set; }

        public bool? Suspended { get; set; }

        public string? Suspendmessage { get; set; }

#nullable enable
        public ResponseCodeDetail? OrderCategory { get; set; }

#nullable enable
        public ResponseCodeDetail? DeliveryMethod { get; set; }

#nullable enable
        public ResponseCodeDetail? ContactID { get; set; }
    }

    public class ResponseCodeDetail
    {
        public string? OrderCategoryCode { get; set; }

        public string? OrderCategoryError { get; set; }

        public string? DeliveryMethodCode { get; set; }

        public string? DeliveryMethodError { get; set; }

        public int? ContactIDCode { get; set; }

        public string? ContactIDError { get; set; }
    }
}