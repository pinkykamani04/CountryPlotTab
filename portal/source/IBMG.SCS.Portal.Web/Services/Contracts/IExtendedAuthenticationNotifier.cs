// Copyright (c) IBMG. All rights reserved.

using PearDrop.Authentication.Contracts;
using ResultMonad;

namespace IBMG.SCS.Portal.Web.Services.Contracts
{
    public interface IExtendedAuthenticationNotifier : IAuthenticationNotifier
    {
        Task<Result> SendCustomSupportEmail(string supportEmail, string subject, string body, CancellationToken cancellationToken = default);
    }
}