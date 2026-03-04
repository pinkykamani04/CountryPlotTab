using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IBMG.SCS.Portal.Web.Client.Services;

namespace IBMG.SCS.Portal.Web.Client.Handlers;

public class KerridgeBranchHandler : DelegatingHandler
{
    private readonly KerridgeBranchService _branchService;

    public KerridgeBranchHandler(KerridgeBranchService branchService)
    {
        _branchService = branchService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_branchService.IsSet)
        {
            Console.WriteLine($"OUTGOING: {request.Method} {request.RequestUri}");
            request.Headers.Remove("X-Branch-Code");
            request.Headers.Add("X-Branch-Code", _branchService.BranchCode);
        }

        return base.SendAsync(request, cancellationToken);
    }
}