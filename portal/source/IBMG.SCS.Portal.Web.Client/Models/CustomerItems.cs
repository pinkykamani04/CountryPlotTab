// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Models
{
    public class CustomerItems
    {
        public Guid CustomerId { get; set; }

        public string AccountNumber { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public double CreditLimit { get; set; }

        public double AvailableCredit { get; set; }
    }
}