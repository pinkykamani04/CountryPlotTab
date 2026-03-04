using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;
using static IBMG.SCS.Portal.Web.Client.Pages.ReportsAnalyticsBase;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetContainerTypesQueryProcessor : IQueryProcessor<GetContainerTypesQuery, GetContainerTypesQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetContainerTypesQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetContainerTypesQueryResult>> Handle(GetContainerTypesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.ContainerTypes
                  .AsNoTracking()
                  .Select(ct => new LookupItemDto
                  {
                      Id = ct.ContainerTypeId,
                      Name = ct.ContainerTypeName
                  })
                  .ToListAsync(cancellationToken);


            return QueryResult<GetContainerTypesQueryResult>.Succeeded(new GetContainerTypesQueryResult(items));
        }
    }
}
