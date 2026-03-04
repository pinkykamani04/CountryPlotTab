// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Branch.Web.Client.Infrastructure.Services;

namespace IBMG.SCS.Branch.Web.Client.Kerridge;

public partial class Client
{
    private readonly KerridgeBranchService _branchService;

    public Client(HttpClient httpClient, KerridgeBranchService branchService)
        : this(httpClient)
    {
        _branchService = branchService;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        if (_branchService.IsSet)
        {
            request.Headers.Remove("X-Branch-Code");
            request.Headers.Add("X-Branch-Code", _branchService.BranchCode.ToString());
        }
    }
}