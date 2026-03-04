// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.KerridgeApi.Server.AppDbContext;
using IBMG.SCS.KerridgeApi.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IBMG.SCS.KerridgeApi.Server.Services;

public class KerridgeRoutingService : IKerridgeRoutingService
{
    private readonly PortalDBContext _db;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "BranchKerridgeMap";

    public KerridgeRoutingService()
    {
    }

    public KerridgeRoutingService(PortalDBContext db, IMemoryCache cache)
    {
        this._db = db;
        this._cache = cache;
    }

    public async Task<string?> GetBaseUrlForBranchAsync(string branchCode)
    {
        var map = await this.GetCachedMapAsync();

        var entry = map.FirstOrDefault(x => x.BranchCode == branchCode);
        return entry?.KerridgeInstance;
    }

    private async Task<List<BranchKerridgeMap>> GetCachedMapAsync()
    {
        var map = await this._cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

            return await this._db.BranchKerridgeMaps.ToListAsync();
        });

        return map ?? new List<BranchKerridgeMap>();
    }
}