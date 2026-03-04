using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;

[BluQubeQuery(Path = "queries/reporting/transactions")]
public record GetReportingTransactionsQuery(
    int? CustomerSiteId,
    int? WasteStreamId,
    int? ContainerTypeId,
    DateTime? StartDate,
    DateTime? EndDate
) : IQuery<GetReportingTransactionsQueryResult>;