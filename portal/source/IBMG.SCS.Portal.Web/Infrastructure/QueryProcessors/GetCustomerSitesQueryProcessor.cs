using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;
using static IBMG.SCS.Portal.Web.Client.Pages.ReportsAnalyticsBase;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetCustomerSitesQueryProcessor : IQueryProcessor<GetCustomerSitesQuery, GetCustomerSitesQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetCustomerSitesQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetCustomerSitesQueryResult>> Handle(GetCustomerSitesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.CustomerSites
               .AsNoTracking()
               .Select(cs => new LookupItemDto
               {
                   Id = cs.CustomerSiteId,
                   Name = cs.CustomerSiteName
               })
               .ToListAsync(cancellationToken);


            return QueryResult<GetCustomerSitesQueryResult>.Succeeded(new GetCustomerSitesQueryResult(items));
        }
    }
}
