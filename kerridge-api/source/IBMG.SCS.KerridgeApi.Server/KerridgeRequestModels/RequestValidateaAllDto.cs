// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestValidateaAllDto(int Level,
        string Levelcode,
        string Custacc,
        string Jobnumbe,
        string Opid,
        string Tradecar);
}