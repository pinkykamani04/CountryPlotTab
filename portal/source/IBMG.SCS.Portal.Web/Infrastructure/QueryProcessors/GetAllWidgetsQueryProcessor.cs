// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllWidgetsQueryProcessor
         : IQueryProcessor<GetAllWidgetsQuery, GetAllWidgetsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllWidgetsQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllWidgetsQueryResult>> Handle(
            GetAllWidgetsQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _context.Widgets
                .Select(w => new GetAllWidgetsQueryResult.WidgetItem(
                    w.Id,
                    w.Name,
                    w.IsMandatory,
                    w.Icon
                ))
                .ToListAsync(cancellationToken);

            return QueryResult<GetAllWidgetsQueryResult>.Succeeded(new GetAllWidgetsQueryResult(items));
        }
    }
}
