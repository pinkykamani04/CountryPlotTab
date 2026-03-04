using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/aircraft-booking/get-all")]
    public record GetAllAircraftBookingQuery(
        Guid? Id,
        Guid? AircraftId,
        Guid? PilotId
    ) : IQuery<GetAllAircraftBookingQueryResult>;
}
