// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllSpendLimitsQueryResult(IReadOnlyList<OperativeDto> limitDtos) : IQueryResult
    {
        public IReadOnlyList<OperativeDto> LimitDtos { get; } = limitDtos;
    }
}