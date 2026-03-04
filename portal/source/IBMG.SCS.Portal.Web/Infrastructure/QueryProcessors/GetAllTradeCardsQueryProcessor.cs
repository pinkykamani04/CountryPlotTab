// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllTradeCardsQueryProcessor : IQueryProcessor<GetAllTradeCardsQuery, GetAllTradeCardsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllTradeCardsQueryProcessor(PortalDBContext context) => _context = context;

        public async Task<QueryResult<GetAllTradeCardsQueryResult>> Handle(GetAllTradeCardsQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.TradeCards
                .Where(x => !x.IsRowDeleted)
                .Select(x => new CardModel
                {
                    Id = x.Id,
                    CardNumber = x.TradeCardNumber,
                    AssigneeId = x.AssigneeId,
                    Status = x.Status,
                    AssigneeName = "" 
                })
                .ToListAsync(cancellationToken);

            var operativeIds = items.Where(i => i.AssigneeId != null).Select(i => i.AssigneeId!.Value).Distinct().ToList();
            var operatives = await _context.Operatives
                .Where(o => operativeIds.Contains(o.Id))
                .Select(o => new { o.Id, FullName = o.FirstName + " " + o.LastName })
                .ToListAsync(cancellationToken);

            foreach (var card in items)
            {
                if (card.AssigneeId.HasValue)
                {
                    var op = operatives.FirstOrDefault(o => o.Id == card.AssigneeId.Value);
                    card.AssigneeName = op?.FullName ?? "-";
                }
                else
                {
                    card.AssigneeName = "-";
                }
            }

            return QueryResult<GetAllTradeCardsQueryResult>.Succeeded(new GetAllTradeCardsQueryResult(items));
        }
    }
}