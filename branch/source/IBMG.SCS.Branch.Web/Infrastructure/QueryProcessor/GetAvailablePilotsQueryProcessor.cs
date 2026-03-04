using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.QueryProcessor
{
    public class GetAvailablePilotsQueryProcessor
        : IQueryProcessor<GetAvailablePilotsQuery, GetAllPilotInformationQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAvailablePilotsQueryProcessor(PortalDBContext context)
        {
            this._context = context;
        }

        public async Task<QueryResult<GetAllPilotInformationQueryResult>> Handle(
            GetAvailablePilotsQuery request,
            CancellationToken cancellationToken)
        {
            var busyPilotIds = await this._context.AircraftBookings
                .Where(b =>
                    !b.IsRowDeleted &&
                    b.FromDate.Add(b.FromTime) < request.ToDateTime &&
                    b.ToDate.Add(b.ToTime) > request.FromDateTime
                )
                .Select(b => b.PilotId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var availablePilots = await this._context.PilotInformation
                .Where(p => !busyPilotIds.Contains(p.Id))
                .Select(p => new GetAllPilotInformationQueryResult.PilotInformationItemDto(
                    p.Id,
                    p.FullName,
                    p.DateOfBirth,
                    p.ContactNumber,
                    p.EmailAddress,
                    p.ContactAddress,
                    p.Location,
                    p.LicenseNumber,
                    p.LicenseType,
                    p.LicenseInsuranceDate,
                    p.LicenseExpiryDate,
                    p.MedicalCertificate,
                    p.ProfileImageName,
                    p.CreatedBy,
                    p.CreatedOn,
                    p.ModifiedBy,
                    p.ModifiedOn
                ))
                .ToListAsync(cancellationToken);

            return QueryResult<GetAllPilotInformationQueryResult>.Succeeded(
                new GetAllPilotInformationQueryResult(availablePilots)
            );
        }
    }
}
