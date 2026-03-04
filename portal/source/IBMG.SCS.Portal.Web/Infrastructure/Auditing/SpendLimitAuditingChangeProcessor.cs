// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Client.Contracts;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.Auditing
{
    //public class SpendLimitAuditingChangeProcessor : ISpendLimitChangeProcessor
    //{
    //    private readonly ICurrentAuthenticatedUserProvider _userProvider;
    //    private readonly IDbContextFactory<PortalDBContext> _dbFactory;

    //    private readonly List<AuditLogs> _pendingLogs = new();

    //    public SpendLimitAuditingChangeProcessor(
    //        ICurrentAuthenticatedUserProvider userProvider,
    //        IDbContextFactory<PortalDBContext> dbFactory)
    //    {
    //        _userProvider = userProvider;
    //        _dbFactory = dbFactory;
    //    }

    //    public Task Gather(DbContext ctx, Guid? commandRef)
    //    {
    //        var entry = ctx.ChangeTracker.Entries<SpendLimits>().FirstOrDefault();
    //        if (entry == null)
    //            return Task.CompletedTask;

    //        var (_, userName) = GetUser();

    //        // NEW SPEND LIMIT CREATED
    //        if (entry.State == EntityState.Added)
    //        {
    //            _pendingLogs.Add(new AuditLogs
    //            {
    //                Id = Guid.NewGuid(),
    //                CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow),
    //                CreatedOnTime = TimeOnly.FromDateTime(DateTime.UtcNow),
    //                User = userName,
    //                Category = "Spend Control",
    //                Action = $"Spend Limit profile created for {entry.Entity.CardNumber}"
    //            });

    //            return Task.CompletedTask;
    //        }

    //        // UPDATED SPEND LIMIT
    //        if (entry.State == EntityState.Modified)
    //        {
    //            foreach (var prop in entry.Properties)
    //            {
    //                if (!prop.IsModified)
    //                    continue;

    //                if (!IsTrackedProperty(prop.Metadata.Name))
    //                    continue;

    //                var oldVal = prop.OriginalValue?.ToString() ?? "null";
    //                var newVal = prop.CurrentValue?.ToString() ?? "null";

    //                _pendingLogs.Add(new AuditLogs
    //                {
    //                    Id = Guid.NewGuid(),
    //                    CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow),
    //                    CreatedOnTime = TimeOnly.FromDateTime(DateTime.UtcNow),
    //                    User = userName,
    //                    Category = GetCategory(prop.Metadata.Name),
    //                    Action = GetActionText(prop.Metadata.Name, oldVal, newVal)
    //                });
    //            }
    //        }

    //        return Task.CompletedTask;
    //    }

    //    public async Task ProcessChanges(Guid? commandRef)
    //    {
    //        if (!_pendingLogs.Any())
    //            return;

    //        await using var db = await _dbFactory.CreateDbContextAsync();
    //        db.AuditLogs.AddRange(_pendingLogs);
    //        await db.SaveChangesAsync();
    //    }

    //    // ✔ TRACK ONLY THE SPEND LIMIT FIELDS
    //    private bool IsTrackedProperty(string propertyName)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => true,
    //            "TnxLimit" => true,
    //            "DailyLimit" => true,
    //            "WeeklyLimit" => true,
    //            "MonthlyLimit" => true,
    //            "EndDate" => true,
    //            _ => false
    //        };
    //    }

    //    private string GetCategory(string propertyName)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => "Status",
    //            _ => "Spend Control"
    //        };
    //    }

    //    private string GetActionText(string propertyName, string oldVal, string newVal)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => $"Status changed to {newVal}",
    //            "TnxLimit" => $"Transaction limit changed to {newVal}",
    //            "DailyLimit" => $"Daily limit changed to {newVal}",
    //            "WeeklyLimit" => $"Weekly limit changed to {newVal}",
    //            "MonthlyLimit" => $"Monthly limit changed to {newVal}",
    //            "EndDate" => $"End date changed to {newVal}",
    //            _ => $"{propertyName} changed from {oldVal} to {newVal}"
    //        };
    //    }

    //    private (Guid, string) GetUser()
    //    {
    //        var current = _userProvider.CurrentAuthenticatedUser;

    //        if (current.HasValue)
    //        {
    //            return current.Value switch
    //            {
    //                AuthenticatedUser u => (u.UserId, $"{u.FirstName} {u.LastName}"),
    //                ISystemUser => (Guid.Empty, "System"),
    //                _ => (Guid.Empty, "Unknown")
    //            };
    //        }

    //        return (Guid.Empty, "Unknown");
    //    }
    //}
}
