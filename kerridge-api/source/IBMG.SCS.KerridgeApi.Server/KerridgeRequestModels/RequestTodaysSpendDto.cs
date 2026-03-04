// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestTodaysSpendDto(int Level,
        string Levelcode,
        string Custacc,
        DateOnly Date);
}