// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Infrastructure.Multitenancy
{
    public interface ITenantExecutionScope
    {
        IDisposable Begin(Guid tenantId);
    }
}