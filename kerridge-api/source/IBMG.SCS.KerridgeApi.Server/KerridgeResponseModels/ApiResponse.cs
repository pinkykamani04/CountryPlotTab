// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }
    }
}