// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestTradeCardValidationDto(int Level,
        string Levelcode,
        string Tradecar,
        string Custacc);
}