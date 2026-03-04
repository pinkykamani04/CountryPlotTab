// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Services
{
    public class KerridgeBranchHandler(KerridgeBranchService branchService) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (branchService.IsSet)
            {
                request.Headers.Remove("X-Branch-Code");
                request.Headers.Add("X-Branch-Code", branchService.BranchCode.ToString());
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}