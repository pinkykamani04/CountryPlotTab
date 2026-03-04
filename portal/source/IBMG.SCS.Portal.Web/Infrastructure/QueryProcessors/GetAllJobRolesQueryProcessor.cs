// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllJobRolesQueryProcessor : IQueryProcessor<GetAllJobRoleQuery, GetAllJobRoleQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllJobRolesQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllJobRoleQueryResult>> Handle(
            GetAllJobRoleQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _context.JobRoles.Where(x => !x.IsRowDeleted)
                                                 .Select(x => new JobRoleDto
                                                 {
                                                     Id = x.Id,
                                                     Name = x.Name,
                                                     UserId = x.UserId,
                                                     UserCount = _context.Operatives.Count(o => o.JobRole == x.Id && !o.IsRowDeleted),
                                                 })
                                                 .ToListAsync(cancellationToken);

            return QueryResult<GetAllJobRoleQueryResult>.Succeeded(new GetAllJobRoleQueryResult(items));
        }
    }
}