// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllSupportEmailQueryProcessor : IQueryProcessor<GetAllSupportEmailQuery, GetAllSupportEmailQueryResult>
    {
        private readonly PortalDBContext _portalDBContext;

        public GetAllSupportEmailQueryProcessor(PortalDBContext portalDBContext)
        {
            this._portalDBContext = portalDBContext;
        }

        public async Task<QueryResult<GetAllSupportEmailQueryResult>> Handle(GetAllSupportEmailQuery request, CancellationToken cancellationToken)
        {
            var supportEmail = await this._portalDBContext.Emails.Where(x => !x.IsRowDeleted)
                                                                 .Select(x => new EmailDto()
                                                                 {
                                                                     Id = x.Id,
                                                                     SupportEmail = x.Email,
                                                                 })
                                                                 .FirstOrDefaultAsync(cancellationToken);

            return QueryResult<GetAllSupportEmailQueryResult>.Succeeded(new GetAllSupportEmailQueryResult(supportEmail));
        }
    }
}