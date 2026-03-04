using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors;

public class GetAllDashboardItemQueryProcessor
    : IQueryProcessor<GetAllDashboardItemQuery, GetAllDashboardItemQueryResult>
{
    private readonly PortalDBContext _context;

    public GetAllDashboardItemQueryProcessor(PortalDBContext context)
    {
        _context = context;
    }

    public async Task<QueryResult<GetAllDashboardItemQueryResult>> Handle(
        GetAllDashboardItemQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _context.DashboardItems
            .Select(x => new GetAllDashboardItemQueryResult.ToDoItem(
                x.Id,
                x.UserId,
                x.CustomerId,
                x.TemplateType
            ))
            .ToListAsync(cancellationToken);

        var result = new GetAllDashboardItemQueryResult(items);

        return QueryResult<GetAllDashboardItemQueryResult>.Succeeded(result);
    }
}
