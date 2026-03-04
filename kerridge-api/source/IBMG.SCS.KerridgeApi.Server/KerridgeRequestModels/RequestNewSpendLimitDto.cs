// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestNewSpendLimitDto(int Level,
        string Levelcode,
        string Custacc,
        decimal Spendlim);
}