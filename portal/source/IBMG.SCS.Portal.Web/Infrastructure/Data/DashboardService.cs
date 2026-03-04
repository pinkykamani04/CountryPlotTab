// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBMG.SCS.Portal.Web.Infrastructure.Data;

public class DashboardService : IDashboardService
{
    private readonly PortalDBContext _context;

    public DashboardService(PortalDBContext context)
    {
        this._context = context;
    }

    public IQueryable<DashboardItem> Items => this._context.DashboardItems.AsQueryable();

    public DashboardItem AddDashboard(Guid id, Guid? userId, Guid? customerId, int templateType)
    {
        var item = new DashboardItem(id, userId, customerId, templateType);
        this._context.DashboardItems.Add(item);
        this._context.SaveChanges();
        return item;
    }

    public DashboardItem? UpdateDashboard(Guid id, Guid? userId, Guid? customerId, int templateType)
    {
        var existing = this._context.DashboardItems.FirstOrDefault(x => x.Id == id);
        if (existing == null)
        {
            return null;
        }

        existing.Update(userId, customerId, templateType);
        this._context.SaveChanges();
        return existing;
    }

    public bool DeleteDashboard(Guid id)
    {
        var item = this._context.DashboardItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return false;
        }

        this._context.DashboardItems.Remove(item);
        this._context.SaveChanges();
        return true;
    }

    public async Task<List<DashboardWidget>> GetWidgetsForDashboardAsync(Guid dashboardId)
    {
        return await this._context.DashboardWidgets
            .Include(w => w.Widget)
            .Where(w => w.DashboardId == dashboardId && !w.IsRowDeleted.GetValueOrDefault())
            .OrderBy(w => w.RowOrder)
            .ThenBy(w => w.Position)
            .ToListAsync();
    }

    public async Task<DashboardWidget> AddWidgetToDashboardAsync(
        Guid id,
        Guid dashboardId,
        int rowOrder,
        string rowLayoutType,
        int position,
        Guid widgetId)
    {
        var widget = await this._context.Widgets
            .FirstOrDefaultAsync(w => w.Id == widgetId);

        if (widget == null)
        {
            return null;
        }

        var dashWidget = new DashboardWidget
        {
            Id = id,
            DashboardId = dashboardId,
            RowOrder = rowOrder,
            RowLayoutType = rowLayoutType,
            Position = position,
            WidgetId = widgetId,
        };

        this._context.DashboardWidgets.Add(dashWidget);
        await this._context.SaveChangesAsync();

        return dashWidget;
    }

    public async Task<bool> RemoveWidgetAsync(Guid dashboardWidgetId)
    {
        var dw = await this._context.DashboardWidgets
            .FirstOrDefaultAsync(w => w.Id == dashboardWidgetId);

        if (dw == null)
        {
            return false;
        }

        dw.IsRowDeleted = true;
        await this._context.SaveChangesAsync();

        return true;
    }

    public async Task<List<DashboardWidget>> UpdateDashboardWidgetLayoutAsync(Guid dashboardId, int rowOrder, string newLayout)
    {
        var widgets = await this._context.DashboardWidgets
    .Where(w =>
                w.RowOrder == rowOrder
                && (w.IsRowDeleted == null || w.IsRowDeleted == false))
    .ToListAsync();

        if (!widgets.Any())
        {
            return [];
        }

        foreach (var widget in widgets)
        {
            widget.RowLayoutType = newLayout;
        }

        await this._context.SaveChangesAsync();
        return widgets;
    }
}