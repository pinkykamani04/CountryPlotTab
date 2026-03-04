using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/spend/summary")]
    public class CustomerSpendSummaryController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerSpendSummary(string customerId)
        {
            var summaries = new[]
            {
                new {
                    customerId = "CUS-3",
                    totalSpend = 450.74,
                    creditLimit = 75000,
                    availableCredit = 74549.26,
                    totalSales = 2
                },
                new {
                    customerId = "CUS-4",
                    totalSpend = 450.00,
                    creditLimit = 75000,
                    availableCredit = 74550.00,
                    totalSales = 1
                }
            };

            var result = summaries.FirstOrDefault(s =>
                s.customerId == customerId
            );

            if (result == null)
                return NotFound("Spend summary not found.");

            return Ok(result);
        }
    }
}
