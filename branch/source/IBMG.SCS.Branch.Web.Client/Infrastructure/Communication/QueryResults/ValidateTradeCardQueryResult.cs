using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults
{
    public record ValidateTradeCardQueryResult(
         bool IsFound,
         bool IsDeactivated,
         TradeValidationCardDto Card) : IQueryResult;
}
