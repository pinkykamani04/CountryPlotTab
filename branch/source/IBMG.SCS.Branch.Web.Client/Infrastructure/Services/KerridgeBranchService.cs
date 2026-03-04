// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Services
{
    public class KerridgeBranchService
    {
        private int? _branchCode;

        public int BranchCode
        {
            get => _branchCode ?? throw new Exception("Branch code not set!");
            set
            {
                _branchCode = value;
                IsSet = _branchCode.HasValue;
            }
        }

        public bool IsSet { get; private set; }
    }
}