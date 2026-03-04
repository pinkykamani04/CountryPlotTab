// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Services
{
    public class KerridgeBranchService
    {
        private string? _branchCode;

        public string BranchCode
        {
            get => _branchCode ?? throw new Exception("Branch code not set!");
            set => _branchCode = value;
        }

        public bool IsSet => _branchCode != null;
    }

}