using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/jobrole/get-all")]

    public record GetAllJobRoleQuery : IQuery<GetAllJobRoleQueryResult>;

}
