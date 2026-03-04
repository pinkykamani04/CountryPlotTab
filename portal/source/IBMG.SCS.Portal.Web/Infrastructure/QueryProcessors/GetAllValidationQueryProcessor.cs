// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.QueryProcessors
{
    public class GetAllValidationQueryProcessor : IQueryProcessor<GetAllValidationQuery, GetAllValidationQueryResult>
    {
        private readonly PortalDBContext _context;

        public GetAllValidationQueryProcessor(PortalDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResult<GetAllValidationQueryResult>> Handle(
            GetAllValidationQuery request,
            CancellationToken cancellationToken)
        {
            var items = await _context.Validations
             .Select(x => new ValidationDto()
             {
                 Id = x.Id,
                 UserId = x.UserId,
                 ValidationType = x.ValidationType,
                 IsEnabled = x.IsEnabled,
                 IsMandatory = x.IsMandatory,
                 CreatedOn = x.CreatedOn,
                 ModifiedOn = x.ModifiedOn,
                 ModifiedBy = x.ModifiedBy,
             })
                .ToListAsync(cancellationToken);

            return QueryResult<GetAllValidationQueryResult>.Succeeded(new GetAllValidationQueryResult(items));
        }
    }
}