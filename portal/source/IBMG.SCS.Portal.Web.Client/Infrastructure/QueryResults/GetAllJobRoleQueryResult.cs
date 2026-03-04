
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetAllJobRoleQueryResult(IReadOnlyList<JobRoleDto> limitDtos) : IQueryResult
    {
        public IReadOnlyList<JobRoleDto> LimitDtos { get; } = limitDtos;
    }
}