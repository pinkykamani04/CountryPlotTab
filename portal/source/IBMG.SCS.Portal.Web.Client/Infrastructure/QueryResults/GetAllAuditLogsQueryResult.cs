using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Models;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.QueryResults
{
    public class GetAllAuditLogsQueryResult(IReadOnlyList<AuditLogsModel> auditLogs) : IQueryResult
    {
        public IReadOnlyList<AuditLogsModel> AuditLogs { get; } = auditLogs;
    }
}
