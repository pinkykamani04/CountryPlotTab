// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllSpendLimitQueryProcessor : IQueryProcessor<GetAllSpendLimitsQuery, GetAllSpendLimitsQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllSpendLimitQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllSpendLimitsQueryResult>> Handle(
            GetAllSpendLimitsQuery request,
            CancellationToken cancellationToken)
        {
            var items = await this._context.Operatives.Where(x => !x.IsRowDeleted)
                                                 .Select(x => new OperativeDto()
                                                 {
                                                     Id = x.Id,
                                                     TradeCardNumber = this._context.TradeCards.FirstOrDefault(y => y.Id == x.TradeCardId && !y.IsRowDeleted).TradeCardNumber,
                                                     FirstName = x.FirstName,
                                                     LastName = x.LastName,
                                                     Status = (Client.Models.Status)x.Status,
                                                     TnxLimit = x.TnxLimit,
                                                     DailyLimit = x.DailyLimit,
                                                     WeeklyLimit = x.WeeklyLimit,
                                                     MonthlyLimit = x.MonthlyLimit,
                                                     OverrideEndDate = x.OverrideEndDate,
                                                     TradeCardId = x.TradeCardId,
                                                     OperativeNumber = x.OperativeNumber,
                                                     OverrideMonthlyLimit = x.OverrideMonthlyLimit,
                                                     OverrideWeeklyLimit = x.OverrideWeeklyLimit,
                                                     OverrideDailyLimit = x.OverrideDailyLimit,
                                                     OverrideTnxLimit = x.OverrideTnxLimit,
                                                     EndDate = x.EndDate,
                                                     JobRole = x.JobRole,
                                                     StartDate = x.StartDate,
                                                 })
                                                 .ToListAsync(cancellationToken);

            return QueryResult<GetAllSpendLimitsQueryResult>.Succeeded(new GetAllSpendLimitsQueryResult(items));
        }
    }
}