using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;
using static IBMG.SCS.Portal.Web.Client.Pages.ReportsAnalyticsBase;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetWasteStreamsQueryProcessor : IQueryProcessor<GetWasteStreamsQuery, GetWasteStreamsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetWasteStreamsQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetWasteStreamsQueryResult>> Handle(GetWasteStreamsQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.WasteStreams
               .AsNoTracking()
               .Select(ws => new LookupItemDto
               {
                   Id = ws.WasteStreamId,
                   Name = ws.WasteStreamName
               })
               .ToListAsync(cancellationToken);


            return QueryResult<GetWasteStreamsQueryResult>.Succeeded(new GetWasteStreamsQueryResult(items));
        }
    }
}
