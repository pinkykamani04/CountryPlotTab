using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetReportingTransactionsQueryProcessor
        : IQueryProcessor<GetReportingTransactionsQuery, GetReportingTransactionsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetReportingTransactionsQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetReportingTransactionsQueryResult>> Handle(
            GetReportingTransactionsQuery request,
            CancellationToken cancellationToken)
        {

            var query = _context.ReportingTransactions
                   .AsNoTracking()

                   .GroupJoin(
                       _context.CustomerSites.AsNoTracking(),
                       rt => rt.CustomerSiteId,
                       cs => cs.CustomerSiteId,
                       (rt, csj) => new { rt, csj }
                   )
                   .SelectMany(
                       x => x.csj.DefaultIfEmpty(),
                       (x, cs) => new { x.rt, cs }
                   )

                   .GroupJoin(
                       _context.WasteStreams.AsNoTracking(),
                       x => x.rt.WasteStreamId,
                       ws => ws.WasteStreamId,
                       (x, wsj) => new { x.rt, x.cs, wsj }
                   )
                   .SelectMany(
                       x => x.wsj.DefaultIfEmpty(),
                       (x, ws) => new { x.rt, x.cs, ws }
                   )

                   .GroupJoin(
                       _context.ContainerTypes.AsNoTracking(),
                       x => x.rt.ContainerTypeId,
                       ct => ct.ContainerTypeId,
                       (x, ctj) => new { x.rt, x.cs, x.ws, ctj }
                   )
                   .SelectMany(
                       x => x.ctj.DefaultIfEmpty(),
                       (x, ct) => new { x.rt, x.cs, x.ws, ct }
                   );

            if (request.CustomerSiteId.HasValue)
                query = query.Where(x => x.rt.CustomerSiteId == request.CustomerSiteId.Value);

            if (request.WasteStreamId.HasValue)
                query = query.Where(x => x.rt.WasteStreamId == request.WasteStreamId.Value);

            if (request.ContainerTypeId.HasValue)
                query = query.Where(x => x.rt.ContainerTypeId == request.ContainerTypeId.Value);

            if (request.StartDate.HasValue)
                query = query.Where(x => x.rt.TransactionDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(x => x.rt.TransactionDate <= request.EndDate.Value);

            var transactions = await query
                .OrderByDescending(x => x.rt.TransactionDate)
                .Select(x => new ReportingTransactionDto
                {
                    Id = x.rt.Id,
                    TransactionId = x.rt.TransactionId,
                    TransactionDate = x.rt.TransactionDate,

                    CustomerSiteId = x.rt.CustomerSiteId,
                    CustomerSiteName = x.cs != null ? x.cs.CustomerSiteName : string.Empty,

                    WasteStreamId = x.rt.WasteStreamId,
                    WasteStreamName = x.ws != null ? x.ws.WasteStreamName : string.Empty,

                    ContainerTypeId = x.rt.ContainerTypeId,
                    ContainerTypeName = x.ct != null ? x.ct.ContainerTypeName : string.Empty,

                    Tonnage = x.rt.Tonnage,
                    NetWeight = x.rt.NetWeight,
                    TransactionValue = x.rt.TransactionValue,
                    TonnageTransaction = x.rt.TonnageTransaction,
                    OrderNumber = x.rt.OrderNumber,
                    TicketNumber = x.rt.TicketNumber,

                    ActivityStatus = x.rt.ActivityStatus,
                    CollectionType = x.rt.CollectionType
                })
                .ToListAsync(cancellationToken);

            return QueryResult<GetReportingTransactionsQueryResult>.Succeeded(
                new GetReportingTransactionsQueryResult(transactions)
            );
        }
    }
}
