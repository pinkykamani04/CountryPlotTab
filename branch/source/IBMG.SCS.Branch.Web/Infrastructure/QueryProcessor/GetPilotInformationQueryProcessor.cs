using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using IBMG.SCS.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Branch.Web.Infrastructure.QueryProcessor
{
    public class GetPilotInformationQueryProcessor
        : IQueryProcessor<GetPilotInformationQuery, GetPilotInformationQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetPilotInformationQueryProcessor(PortalDBContext context)
        {
            this._context = context;
        }

        public async Task<QueryResult<GetPilotInformationQueryResult>> Handle(
            GetPilotInformationQuery request,
            CancellationToken cancellationToken)
        {
            var query = this._context.PilotInformation
                                .AsQueryable();

            if (request.Id.HasValue)
            {
                query = query.Where(p => p.Id == request.Id.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.EmailAddress))
            {
                query = query.Where(p => p.EmailAddress == request.EmailAddress);
            }

            var items = await query
                .Select(p => new GetPilotInformationQueryResult.PilotInformationItemDto(
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

            return QueryResult<GetPilotInformationQueryResult>.Succeeded(
                new GetPilotInformationQueryResult(items)
            );
        }
    }
}
