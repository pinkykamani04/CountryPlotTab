// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults;

public record GetAllDashboardItemQueryResult(IReadOnlyList<GetAllDashboardItemQueryResult.ToDoItem> ToDoItems) : IQueryResult
{
    public record ToDoItem(Guid Id, Guid? UserId, Guid? CustomerId, int TemplateType);

}