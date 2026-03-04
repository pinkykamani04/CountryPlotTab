using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/spend")]
    public class CustomerSpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerSpend(string customerId)
        {
            var spend = new[]
            {
                new {
                    customerId = "CUS-1",
                    orderId = "ORD-101",
                    amount = 150.75,
                    date = DateTime.UtcNow.AddDays(-10)
                },
                new {
                    customerId = "CUS-1",
                    orderId = "ORD-102",
                    amount = 299.99,
                    date = DateTime.UtcNow.AddDays(-5)
                },
                new {
                    customerId = "CUS-2",
                    orderId = "ORD-201",
                    amount = 450.00,
                    date = DateTime.UtcNow.AddDays(-7)
                }
            };

            var result = spend.Where(s =>
                s.customerId == customerId
            ).ToList();

            return Ok(result);
        }
    }
}
