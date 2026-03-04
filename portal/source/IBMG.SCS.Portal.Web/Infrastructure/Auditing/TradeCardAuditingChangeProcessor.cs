using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Client.Contracts;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.Auditing
{
    //public class TradeCardAuditingChangeProcessor : ITradeCardChangeProcessor
    //{
    //    private readonly ICurrentAuthenticatedUserProvider _userProvider;
    //    private readonly IDbContextFactory<PortalDBContext> _dbFactory;

    //    private readonly List<AuditLogs> _pendingLogs = new();

    //    public TradeCardAuditingChangeProcessor(
    //        ICurrentAuthenticatedUserProvider userProvider,
    //        IDbContextFactory<PortalDBContext> dbFactory)
    //    {
    //        _userProvider = userProvider;
    //        _dbFactory = dbFactory;
    //    }

    //    public Task Gather(DbContext ctx, Guid? commandRef)
    //    {
    //        var entry = ctx.ChangeTracker.Entries<TradeCards>().FirstOrDefault();
    //        if (entry == null)
    //            return Task.CompletedTask;

    //        var (_, userName) = GetUser();

    //        // Handle NEW trade card creation
    //        if (entry.State == EntityState.Added)
    //        {
    //            _pendingLogs.Add(new AuditLogs
    //            {
    //                Id = Guid.NewGuid(),
    //                CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow),
    //                CreatedOnTime = TimeOnly.FromDateTime(DateTime.UtcNow),
    //                User = userName,
    //                Category = "Card",
    //                Action = $"New TradeCard created: {entry.Entity.TradeCardNumber}"
    //            });

    //            return Task.CompletedTask;
    //        }

    //        // Handle CHANGES (but only the selected fields)
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

    //    private bool IsTrackedProperty(string propertyName)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => true,
    //            "AssigneeId" => true,
    //            "TradeCardNumber" => true,
    //            _ => false // ignore everything else
    //        };
    //    }

    //    private string GetCategory(string propertyName)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => "Status",
    //            "AssigneeId" => "Role",
    //            "TradeCardNumber" => "Card",
    //            _ => "TradeCard"
    //        };
    //    }

    //    private string GetActionText(string propertyName, string oldVal, string newVal)
    //    {
    //        return propertyName switch
    //        {
    //            "Status" => $"TradeCard status changed to {newVal}",
    //            "AssigneeId" => $"TradeCard role changed to {newVal}",
    //            "TradeCardNumber" => $"TradeCard number changed to {newVal}",
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
