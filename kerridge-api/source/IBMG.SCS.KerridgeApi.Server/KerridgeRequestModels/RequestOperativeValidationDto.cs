// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestOperativeValidationDto(int Level,
        string Levelcode,
        string Opid,
        string Custacc);
}