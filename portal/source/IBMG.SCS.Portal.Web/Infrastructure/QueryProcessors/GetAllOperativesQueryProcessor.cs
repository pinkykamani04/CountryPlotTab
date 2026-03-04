// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllOperativesQueryProcessor : IQueryProcessor<GetAllOperativesQuery, GetAllOperativeQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllOperativesQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllOperativeQueryResult>> Handle(
            GetAllOperativesQuery request,
            CancellationToken cancellationToken)
        {
            var items = await this._context.Operatives.Where(x => !x.IsRowDeleted)
                                                      .Select(x => new OperativeDto()
                                                      {
                                                          Id = x.Id,
                                                          FirstName = x.FirstName,
                                                          LastName = x.LastName,
                                                          EndDate = x.EndDate,
                                                          JobRole = x.JobRole,
                                                          OperativeNumber = x.OperativeNumber,
                                                          DailyLimit = x.DailyLimit,
                                                          MonthlyLimit = x.MonthlyLimit,
                                                          TnxLimit = x.TnxLimit,
                                                          WeeklyLimit = x.WeeklyLimit,
                                                          StartDate = x.StartDate,
                                                          Status = (Client.Models.Status)x.Status,
                                                          TradeCardId = this._context.TradeCards.Where(c => c.AssigneeId == x.Id && !c.IsRowDeleted)
                                                                                                .OrderByDescending(c => c.ModifiedOn ?? c.CreatedOn)
                                                                                                .Select(c => c.Id)
                                                                                                .FirstOrDefault(),
                                                      })
                                                      .ToListAsync(cancellationToken);

            return QueryResult<GetAllOperativeQueryResult>.Succeeded(new GetAllOperativeQueryResult(items));
        }
    }
}