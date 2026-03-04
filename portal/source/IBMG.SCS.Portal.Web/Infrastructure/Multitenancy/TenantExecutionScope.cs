// Copyright (c) IBMG. All rights reserved.

using Finbuckle.MultiTenant.Abstractions;
using PearDrop.Multitenancy;

namespace IBMG.SCS.Portal.Web.Infrastructure.Multitenancy
{
    public sealed class TenantExecutionScope : ITenantExecutionScope
    {
        private readonly IMultiTenantContextAccessor<PearDropTenantInfo> _accessor;
        private readonly IMultiTenantContextSetter _setter;

        public TenantExecutionScope(
            IMultiTenantContextAccessor<PearDropTenantInfo> accessor,
            IMultiTenantContextSetter setter)
        {
            _accessor = accessor;
            _setter = setter;
        }

        public IDisposable Begin(Guid tenantId)
        {
            var originalContext = _accessor.MultiTenantContext;

            // You already know how to load tenant info (DB, cache, etc.)
            var tenantInfo = new PearDropTenantInfo(
                tenantId.ToString(),
                tenantId.ToString(),
                tenantId.ToString());

            _setter.MultiTenantContext = new MultiTenantContext<PearDropTenantInfo>(tenantInfo);

            return new Scope(() =>
            {
                _setter.MultiTenantContext = originalContext;
            });
        }

        private sealed class Scope : IDisposable
        {
            private readonly Action _onDispose;
            public Scope(Action onDispose) => _onDispose = onDispose;
            public void Dispose() => _onDispose();
        }
    }

}
