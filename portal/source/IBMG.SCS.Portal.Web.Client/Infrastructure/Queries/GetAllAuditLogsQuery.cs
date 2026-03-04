using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Queries
{
    [BluQubeQuery(Path = "queries/auditlog/get-all")]
    public record GetAllAuditLogsQuery : IQuery<GetAllAuditLogsQueryResult>;
}