// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class OrderLineDetails
    {
        public string Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }
    }

}