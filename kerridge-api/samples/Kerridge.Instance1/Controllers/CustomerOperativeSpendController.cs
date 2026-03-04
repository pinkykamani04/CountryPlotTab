using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/spend/operative/{operativeId}")]
    public class CustomerOperativeSpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerOperativeSpend(string customerId, string operativeId)
        {
            var spends = new[]
            {
                new {
                    customerId = "CUS-1",
                    operativeId = "OP-1",
                    operativeName = "John Doe",
                    totalSpend = 1200.50m,
                    currency = "GBP",
                    startDate = DateTime.UtcNow.AddMonths(-2),
                    endDate = DateTime.UtcNow,
                    notes = "Regular monthly purchases"
                },
                new {
                    customerId = "CUS-2",
                    operativeId = "OP-2",
                    operativeName = "Anna Miller",
                    totalSpend = 845.10m,
                    currency = "GBP",
                    startDate = DateTime.UtcNow.AddMonths(-1),
                    endDate = DateTime.UtcNow,
                    notes = "High-value items"
                },
                new {
                    customerId = "CUS-3",
                    operativeId = "OP-3",
                    operativeName = "Mark Lee",
                    totalSpend = 450.00m,
                    currency = "GBP",
                    startDate = DateTime.UtcNow.AddMonths(-3),
                    endDate = DateTime.UtcNow.AddDays(-5),
                    notes = "Occasional orders"
                }
            };

            var result = spends.FirstOrDefault(s =>
                s.customerId == customerId &&
                s.operativeId == operativeId
            );

            if (result == null)
                return NotFound("Operative spend details not found.");

            return Ok(result);
        }
    }
}
