// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestCustomerListDto(
        int Level,
        string Levelcode,
        string Branchco);
}