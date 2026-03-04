using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customers/{customerId}/operative/trade-card/{tradeCardNumber}")]
    public class OperativeValidationByTradeCardController : ControllerBase
    {
        [HttpGet]
        public IActionResult ValidateOperative(string customerId, string tradeCardNumber)
        {
            var operatives = new[]
            {
                new {
                    customerId = "CUS-3",
                    tradeCardNumber = "14564432",
                    firstName = "John",
                    surname = "Doe",
                    valid = 1
                },
                new {
                    customerId = "CUS-3",
                    tradeCardNumber = "131232323232",
                    firstName = "Emma",
                    surname = "Stone",
                    valid = 1
                },
                new {
                    customerId = "CUS-4",
                    tradeCardNumber = "99999999",
                    firstName = "Invalid",
                    surname = "User",
                    valid = 0
                }
            };

            var result = operatives.FirstOrDefault(o =>
                o.customerId == customerId && o.tradeCardNumber == tradeCardNumber
            );

            if (result == null)
                return NotFound("Not found.");

            return Ok(result);
        }
    }
}
