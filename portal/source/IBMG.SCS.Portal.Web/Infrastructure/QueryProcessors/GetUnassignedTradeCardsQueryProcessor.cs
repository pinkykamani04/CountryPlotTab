// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetUnassignedTradeCardsQueryProcessor
        : IQueryProcessor<GetUnassignedTradeCardsQuery, GetUnassignedTradeCardsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetUnassignedTradeCardsQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetUnassignedTradeCardsQueryResult>> Handle(
            GetUnassignedTradeCardsQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _context.TradeCards
                .Where(x => !x.IsRowDeleted && x.AssigneeId == Guid.Empty)
                .Select(x => new CardModel
                {
                    Id = x.Id,
                    CardNumber = x.TradeCardNumber,
                    AssigneeId = x.AssigneeId,
                    Status = x.Status,
                    AssigneeName = "-" , 
                })
                .ToListAsync(cancellationToken);

            return QueryResult<GetUnassignedTradeCardsQueryResult>
                .Succeeded(new GetUnassignedTradeCardsQueryResult(items));
        }
    }
}