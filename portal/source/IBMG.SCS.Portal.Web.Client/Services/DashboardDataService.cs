using IBMG.SCS.Portal.ApiClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static IBMG.SCS.Portal.Web.Client.Components.Widgets.WidgetsPopup;

namespace IBMG.SCS.Portal.Web.Client.Services
{
    public class DashboardDataService
    {
        private readonly IClient client;
        private readonly IMemoryCache cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        ILogger<DashboardDataService> logger;

        public DashboardDataService(IClient client, IMemoryCache cache, ILogger<DashboardDataService> logger)
        {
            this.client = client;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<double?> GetFaultyGoodsKpiAsync(string customerId)
        {
            var cacheKey = $"kpi:faulty:{customerId}";

            if (cache.TryGetValue(cacheKey, out double? cachedValue))
            {
                return cachedValue;
            }

            var response = await client.GetKpiFaultyGoodsAsync(customerId);
            var value = ExtractKpiValue(response?.Data);

            cache.Set(cacheKey, value, CacheDuration);
            return value;
        }

        public async Task<double?> GetOtifKpiAsync(string customerId)
        {
            var cacheKey = $"kpi:otif:{customerId}";

            if (cache.TryGetValue(cacheKey, out double? cachedValue))
            {
                return cachedValue;
            }

            var response = await client.GetKpiOtifAsync(customerId);
            var value = ExtractKpiValue(response?.Data);

            cache.Set(cacheKey, value, CacheDuration);
            return value;
        }

        public async Task<double?> GetInvoiceAccuracyKpiAsync(string customerId)
        {
            var cacheKey = $"kpi:invoice:{customerId}";

            if (cache.TryGetValue(cacheKey, out double? cachedValue))
            {
                return cachedValue;
            }

            var response = await client.GetKpiInvoiceAccuracyAsync(customerId);
            var value = ExtractKpiValue(response?.Data);

            cache.Set(cacheKey, value, CacheDuration);
            return value;
        }

        public async Task<List<NameAmount>> GetSpendByOperativeAsync( int customerId, string filter)
        {
            var cacheKey = $"spend:operative:{customerId}:{filter}";

            if (cache.TryGetValue(cacheKey, out List<NameAmount> cached))
            {
                return cached;
            }

            var response = await client.GetOpertaiveBySpendAsync(customerId, filter);

            var data = response?.Data == null
                ? new List<NameAmount>()
                : response.Data
                    .Select(x => new NameAmount(
                        Text: x.OperativeName,
                        Amount: decimal.TryParse(x.OperativeSpend.ToString().Replace("£", ""), out var amt) ? amt : 0
                    ))
                    .OrderByDescending(x => x.Amount)
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<CategorySpend>> GetSpendByCategoryAsync(int customerId, string filter)
        {
            var cacheKey = $"spend:category:{filter}";

            if (cache.TryGetValue(cacheKey, out List<CategorySpend> cached))
            {
                return cached;
            }

            var response = await client.GetCategoryBySpendAsync(customerId, filter);

            var data = response?.Data == null
                ? new List<CategorySpend>()
                : response.Data
                    .Select(x => new CategorySpend(
                        Cat: x.CategoryName,
                        Amount: CleanAndParse(x.CategorySpend.ToString())
                    ))
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<RankedItem>> GetTopOperativesBySpendAsync(int customerId, string filter)
        {
            var normalizedFilter = NormalizeFilter(filter);
            var cacheKey = $"top:operative:{customerId}:{normalizedFilter}";

            if (cache.TryGetValue(cacheKey, out List<RankedItem> cached))
            {
                return cached;
            }

            var response =
                await client.GetOpertaiveBySpendAsync(customerId, normalizedFilter);

            var data = response?.Data == null
                ? new()
                : response.Data
                    .Select((x, index) => new RankedItem(
                        Rank: index + 1,
                        Name: x.OperativeName,
                        Amount: decimal.TryParse(
                            x.OperativeSpend?.ToString()?.Replace("£", ""),
                            out var amt) ? amt : 0
                    ))
                    .OrderByDescending(x => x.Amount)
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<NameAmount>> GetSpendByBranchAsync(int customerId, string filter)
        {
            var normalizedFilter = NormalizeFilter(filter);
            var cacheKey = $"spend:branch:{customerId}:{normalizedFilter}";

            if (cache.TryGetValue(cacheKey, out List<NameAmount> cached))
            {
                return cached;
            }

            var response =
                await client.GetBranchesBySpendAsync(customerId, normalizedFilter);

            var data = response?.Data == null
                ? new()
                : response.Data
                    .Select(x => new NameAmount(
                        Text: x.BranchName,
                        Amount: decimal.TryParse(
                            x.BranchSpend?.ToString()?.Replace("£", ""),
                            out var amt) ? amt : 0
                    ))
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<RankedItem>> GetTopBranchesBySpendAsync(int customerId, string filter)
        {
            var normalizedFilter = NormalizeFilter(filter);
            var cacheKey = $"top:branch:{customerId}:{normalizedFilter}";

            if (cache.TryGetValue(cacheKey, out List<RankedItem> cached))
            {
                return cached;
            }

            var response =
                await client.GetBranchesBySpendAsync(customerId, normalizedFilter);

            var data = response?.Data == null
                ? new()
                : response.Data
                    .Select((x, index) => new RankedItem(
                        Rank: index + 1,
                        Name: x.BranchName,
                        Amount: decimal.TryParse(
                            x.BranchSpend?.ToString()?.Replace("£", ""),
                            out var amt) ? amt : 0
                    ))
                    .OrderByDescending(x => x.Amount)
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<RankedItem>> GetTopProductsBySpendAsync(int customerId, string filter)
        {
            var normalizedFilter = NormalizeFilter(filter);
            var cacheKey = $"top:product:{customerId}:{normalizedFilter}";

            if (cache.TryGetValue(cacheKey, out List<RankedItem> cached))
            {
                return cached;
            }

            var response =
                await client.GetProductsBySpendAsync(customerId, normalizedFilter);

            var data = response?.Data == null
                ? new()
                : response.Data
                    .Select(x => new
                    {
                        x.ProductName,
                        Amount = decimal.TryParse(
                            x.ProductSpend?.ToString()?.Replace("£", ""),
                            out var amt) ? amt : 0
                    })
                    .OrderByDescending(x => x.Amount)
                    .Take(5)
                    .Select((x, index) => new RankedItem(
                        Rank: index + 1,
                        Name: x.ProductName,
                        Amount: x.Amount
                    ))
                    .ToList();

            cache.Set(cacheKey, data, CacheDuration);
            return data;
        }

        private decimal CleanAndParse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Replace("£", "").Replace(",", "").Trim();
            return decimal.TryParse(value, out var amt) ? amt : 0;
        }

        private double? ExtractKpiValue(object data)
        {
            if (data == null)
                return null;

            var type = data.GetType();
            var props = new[] { "Value", "KpiValue", "Percentage", "Amount", "Total" };

            foreach (var name in props)
            {
                var prop = type.GetProperty(name);
                if (prop != null &&
                    double.TryParse(prop.GetValue(data)?.ToString(), out var result))
                {
                    return result;
                }
            }

            return null;
        }

        public string NormalizeFilter(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return "year to date"; 

            return filter.Trim().ToLower() switch
            {
                "month to date" => "month to date",
                "year to date" => "year to date",
                "last year" => "last year",
                "last year to date" => "last year to date",
                _ => "year to date" 
            };
        }

    }

    public record NameAmount(string Text, decimal Amount);


}
