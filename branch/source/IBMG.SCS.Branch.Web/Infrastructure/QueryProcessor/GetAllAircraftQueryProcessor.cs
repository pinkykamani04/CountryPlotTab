using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.QueryProcessor
{
    public class GetAllAircraftQueryProcessor : IQueryProcessor<GetAllAircraftQuery, GetAllAircraftQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllAircraftQueryProcessor(PortalDBContext context)
        {
            this._context = context;
        }

        public async Task<QueryResult<GetAllAircraftQueryResult>> Handle(
            GetAllAircraftQuery request,
            CancellationToken cancellationToken)
        {
            var query = this._context.AircraftInfo
                                .Where(a => !a.IsRowDeleted)
                                .AsQueryable();

            if (request.Id.HasValue)
            {
                query = query.Where(a => a.Id == request.Id.Value);
            }

            if (request.AircraftId.HasValue)
            {
                query = query.Where(a => a.AircraftId == request.AircraftId.Value);
            }

            var items = await query
                .Select(a => new GetAllAircraftQueryResult.AircraftItemDto(
                    a.Id,
                    a.AircraftId,
                    a.TailNumber,
                    a.Location,
                    a.Model,
                    a.YearOfManufacture,
                    a.Manufacturer,
                    a.AircraftType.ToString(),
                    a.VariantType,
                    a.TypeRating,
                    a.IsRowDeleted,
                    a.CreatedBy,
                    a.CreatedOn,
                    a.ModifiedBy,
                    a.ModifiedOn
                ))
                .ToListAsync(cancellationToken);

            return QueryResult<GetAllAircraftQueryResult>.Succeeded(
                new GetAllAircraftQueryResult(items)
            );
        }
    }
}