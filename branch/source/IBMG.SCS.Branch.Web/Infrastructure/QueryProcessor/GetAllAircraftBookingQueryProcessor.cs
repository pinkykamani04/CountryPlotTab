using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.QueryProcessor
{
    public class GetAllAircraftBookingQueryProcessor : IQueryProcessor<GetAllAircraftBookingQuery, GetAllAircraftBookingQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllAircraftBookingQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllAircraftBookingQueryResult>> Handle(
            GetAllAircraftBookingQuery request,
            CancellationToken cancellationToken)
        {
            var query = this._context.AircraftBookings
                                .Where(b => !b.IsRowDeleted)
                                .AsQueryable();
            if (request.Id.HasValue)
            {
                query = query.Where(b => b.Id == request.Id.Value);
            }

            if (request.AircraftId.HasValue)
            {
                query = query.Where(b => b.AircraftId == request.AircraftId.Value);
            }

            var bookings = await query
                .Select(b => new GetAllAircraftBookingQueryResult.AircraftBookingItemDto(
                    b.Id,
                    b.AircraftId,
                    b.PilotId,
                    b.TailNumber,
                    b.FromDate,
                    b.FromTime,
                    b.ToDate,
                    b.ToTime,
                    b.FromLocation,
                    b.ToLocation,
                    b.Status,
                    b.IsRowDeleted,
                    b.CreatedBy,
                    b.CreatedOn,
                    b.ModifiedBy,
                    b.ModifiedOn
                ))
                .ToListAsync(cancellationToken);

            return QueryResult<GetAllAircraftBookingQueryResult>.Succeeded(
                new GetAllAircraftBookingQueryResult(bookings)
            );
        }
    }
}
