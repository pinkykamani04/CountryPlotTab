// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.QueryResults;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.QueryProcessors
{
    public class GetAllAuditLogsQueryProcessor
      : IQueryProcessor<GetAllAuditLogsQuery, GetAllAuditLogsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllAuditLogsQueryProcessor(PortalDBContext context)
        {
            this._context = context;
        }

        public async Task<QueryResult<GetAllAuditLogsQueryResult>> Handle(
            GetAllAuditLogsQuery request,
            CancellationToken cancellationToken)
        {
            var items = await this._context.AuditLogs
                                                 .Select(x => new AuditLogsModel()
                                                 {
                                                     Id = x.Id,
                                                     Date = x.CreatedOnDate,
                                                     Time = x.CreatedOnTime,
                                                     User = x.User,
                                                     Category = x.Category,
                                                     Action = x.Action,
                                                 })
                                                 .OrderByDescending(x => x.Date.ToDateTime(x.Time))
                                                 .ToListAsync(cancellationToken);

            return QueryResult<GetAllAuditLogsQueryResult>.Succeeded(new GetAllAuditLogsQueryResult(items));
        }
    }
}