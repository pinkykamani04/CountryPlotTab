using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using static IBMG.SCS.Portal.Web.Client.Pages.ReportsAnalyticsBase;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.QueryResults
{
    public class GetWasteStreamsQueryResult(IReadOnlyList<LookupItemDto> items) : IQueryResult
    {
        public IReadOnlyList<LookupItemDto> Items { get; } = items;
    }
}
