// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using PearDrop.Authentication.Client.Contracts;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.Auditing
{
    /*
    Plan (pseudocode):
    - Implement the generic interface properly by supplying the concrete DbContext type.
    - Update the Gather method signature to accept the concrete `PortalDBContext` instead of `DbContext`.
    - Keep all existing logic unchanged so behavior remains identical.
    - Ensure ProcessChanges still uses IDbContextFactory<PortalDBContext> to persist pending logs.
    */

    public class OperativeAuditingChangeProcessor : IOperativeChangeProcessor<PortalDBContext>
    {
        private readonly ICurrentAuthenticatedUserProvider _userProvider;
        private readonly IDbContextFactory<PortalDBContext> _dbFactory;

        private readonly List<AuditLogs> _pendingLogs = new();

        public OperativeAuditingChangeProcessor(
            ICurrentAuthenticatedUserProvider userProvider,
            IDbContextFactory<PortalDBContext> dbFactory)
        {
            this._userProvider = userProvider;
            this._dbFactory = dbFactory;
        }

        public Task Gather(PortalDBContext ctx, Guid? commandRef)
        {
            var entry = ctx.ChangeTracker.Entries<Operatives>().FirstOrDefault();
            if (entry == null)
                return Task.CompletedTask;

            var (_, userName) = this.GetUser();

            if (entry.State == EntityState.Added)
            {
                this._pendingLogs.Add(new AuditLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedOnTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                    User = userName,
                    Category = "Operative",
                    Action = $"New operative created: {entry.Entity.FirstName} {entry.Entity.LastName}",
                });

                return Task.CompletedTask;
            }

            if (entry.State == EntityState.Modified)
            {
                foreach (var prop in entry.Properties)
                {
                    if (!prop.IsModified)
                    {
                        continue;
                    }

                    if (!this.IsTrackedProperty(prop.Metadata.Name))
                    {
                        continue;
                    }

                    var oldVal = prop.OriginalValue?.ToString() ?? "null";
                    var newVal = prop.CurrentValue?.ToString() ?? "null";

                    this._pendingLogs.Add(new AuditLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedOnDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedOnTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                        User = userName,
                        Category = this.GetCategory(prop.Metadata.Name),
                        Action = this.GetActionText(prop.Metadata.Name, oldVal, newVal),
                    });
                }
            }

            return Task.CompletedTask;
        }

        public async Task ProcessChanges(Guid? commandRef)
        {
            if (!this._pendingLogs.Any())
            { return; }

            await using var db = await this._dbFactory.CreateDbContextAsync();
            db.AuditLogs.AddRange(this._pendingLogs);
            await db.SaveChangesAsync();
        }

        private bool IsTrackedProperty(string propertyName)
        {
            return propertyName switch
            {
                "FirstName" => true,
                "LastName" => true,
                "JobRole" => true,
                "OperativeNumber" => true,
                "StartDate" => true,
                "EndDate" => true,
                "Status" => true,
                _ => false,
            };
        }

        private string GetCategory(string propertyName)
        {
            return propertyName switch
            {
                "FirstName" => "Operative",
                "LastName" => "Operative",
                "JobRole" => "Role",
                "OperativeNumber" => "Operative",
                "StartDate" => "Schedule",
                "EndDate" => "Schedule",
                "Status" => "Status",
                _ => "Operative",
            };
        }

        private string GetActionText(string propertyName, string oldVal, string newVal)
        {
            return propertyName switch
            {
                "FirstName" => $"Operative first name changed to {newVal}",
                "LastName" => $"Operative last name changed to {newVal}",
                "JobRole" => $"Operative role changed to {newVal}",
                "OperativeNumber" => $"Operative number changed to {newVal}",
                "StartDate" => $"Operative start date changed to {newVal}",
                "EndDate" => $"Operative end date changed to {newVal}",
                "Status" => $"Operative status changed to {newVal}",
                _ => $"{propertyName} changed from {oldVal} to {newVal}",
            };
        }

        private (Guid, string) GetUser()
        {
            var current = this._userProvider.CurrentAuthenticatedUser;

            if (current.HasValue)
            {
                return current.Value switch
                {
                    AuthenticatedUser u => (u.UserId, $"{u.FirstName} {u.LastName}"),
                    ISystemUser => (Guid.Empty, "System"),
                    _ => (Guid.Empty, "Unknown"),
                };
            }

            return (Guid.Empty, "Unknown");
        }
    }
}