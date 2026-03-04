using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/lookups/container-types")]
    public record GetContainerTypesQuery() : IQuery<GetContainerTypesQueryResult>;
}
