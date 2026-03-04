using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/aircraft/get-all")]
    public record GetAllAircraftQuery(Guid? Id , Guid? AircraftId) : IQuery<GetAllAircraftQueryResult>;
}
