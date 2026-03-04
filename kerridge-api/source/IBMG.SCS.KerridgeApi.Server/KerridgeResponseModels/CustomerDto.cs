// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }

        public string[] AccountNumber { get; set; } = Array.Empty<string>();

        public string Name { get; set; }

        public bool Active { get; set; }

        public decimal CreditLimit { get; set; }

        public decimal AvailableCredit { get; set; }
    }
}