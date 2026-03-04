using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllDashboardWidgetQueryProcessor : IQueryProcessor<GetAllDashboardWidgetQuery, GetAllDashboardWidgetQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllDashboardWidgetQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllDashboardWidgetQueryResult>> Handle(
            GetAllDashboardWidgetQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _context.DashboardWidgets
                .Select(x => new GetAllDashboardWidgetQueryResult.WidgetItems(
                  x.Id,
                  x.DashboardId,
                  x.RowOrder,
                  x.RowLayoutType,
                  x.Position,
                  x.WidgetId,
                  x.IsRowDeleted
                ))
                .ToListAsync(cancellationToken);

            var result = new GetAllDashboardWidgetQueryResult
            {
                Items = items,
            };
            return QueryResult<GetAllDashboardWidgetQueryResult>.Succeeded(result);
        }
    }
}