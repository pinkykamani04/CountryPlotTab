// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.Entities;

namespace IBMG.SCS.Portal.Web.Infrastructure.Data
{
    public interface IDashboardService
    {
        IQueryable<DashboardItem> Items { get; }

        DashboardItem AddDashboard(Guid id, Guid? userId, Guid? customerId, int templateType);

        DashboardItem UpdateDashboard(Guid id, Guid? userId, Guid? customerId, int templateType);

        bool DeleteDashboard(Guid id);

        Task<DashboardWidget> AddWidgetToDashboardAsync(Guid id, Guid dashboardId, int rowOrder, string rowLayoutType, int position, Guid widgetId);

        Task<List<DashboardWidget>> GetWidgetsForDashboardAsync(Guid dashboardId);

        Task<bool> RemoveWidgetAsync(Guid dashboardWidgetId);

        Task<List<DashboardWidget>> UpdateDashboardWidgetLayoutAsync(Guid dashboardId, int rowOrder, string newLayout);
    }
}