// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Data.Querier.Tenants
{
    public class QueryableTenant : IQueryResult
    {
        public Guid Id { get; set; }

        public string Identifier { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool IsDisabled { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? WhenDisabled { get; set; }

        public int? DeviceRemembranceExpirationInDays { get; set; }

        public bool? UseDeviceRemembrance { get; set; }
    }
}