// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllSupportEmailQueryResult(EmailDto emailDtos) : IQueryResult
    {
        public EmailDto EmailDtos { get; set; } = emailDtos;
    }
}