// Copyright (c) IBMG. All rights reserved.

using PearDrop.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.Auditing
{
    public interface IOperativeChangeProcessor<TDbContext> : IChangeProcessor<TDbContext>
        where TDbContext : DbContext
    {
    }
}