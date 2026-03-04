// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.Services;

public interface IKerridgeRoutingService
{
    Task<string?> GetBaseUrlForBranchAsync(string branchCode);
}