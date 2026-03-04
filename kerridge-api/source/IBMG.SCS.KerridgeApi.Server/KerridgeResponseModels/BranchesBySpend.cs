// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels
{
    public class BranchesBySpend
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public decimal? BranchSpend { get; set; }
    }
}