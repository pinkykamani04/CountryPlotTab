// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllOperativeQueryResult(IReadOnlyList<OperativeDto> operativeDtos) : IQueryResult
    {
        public IReadOnlyList<OperativeDto> OperativeDtos { get; } = operativeDtos;
    }
}