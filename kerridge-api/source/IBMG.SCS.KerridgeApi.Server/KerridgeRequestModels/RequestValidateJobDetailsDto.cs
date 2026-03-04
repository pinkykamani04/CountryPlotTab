// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels
{
    public record RequestValidateJobDetailsDto(int Level,
        string Levelcode,
        string Custacc,
        string Jobnum);
}