using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
        public class GetReportingTransactionsQueryResult(
             IReadOnlyList<ReportingTransactionDto> transactions
         ) : IQueryResult
        {
            public IReadOnlyList<ReportingTransactionDto> Transactions { get; } = transactions;
        }
}
