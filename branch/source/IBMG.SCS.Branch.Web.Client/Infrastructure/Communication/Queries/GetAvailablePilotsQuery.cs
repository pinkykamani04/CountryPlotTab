using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/pilot/get-available")]
    public record GetAvailablePilotsQuery(
        DateTime FromDateTime,
        DateTime ToDateTime
    ) : IQuery<GetAllPilotInformationQueryResult>;
}
