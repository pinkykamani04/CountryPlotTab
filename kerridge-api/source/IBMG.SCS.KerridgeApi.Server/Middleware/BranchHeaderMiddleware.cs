// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.KerridgeApi.Server.Middleware;

public class BranchHeaderMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Branch-Code", out var branchCode))
        {
            context.Items["BranchCode"] = branchCode.ToString();
        }
        else
        {
            context.Items["BranchCode"] = null;
        }

        await next(context);
    }
}