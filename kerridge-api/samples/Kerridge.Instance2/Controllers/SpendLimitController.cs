using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/spend-limit")]
    public class SpendLimitController : ControllerBase
    {
        [HttpPut]
        public IActionResult UpdateSpendLimit(string customerId, [FromQuery] decimal newLimit)
        {
            var customers = new[]
            {
                "CUS-3",
                "CUS-4"
            };

            if (!customers.Contains(customerId))
                return NotFound("Customer not found.");

            var response = new
            {
                customerId,
                newSpendLimit = newLimit
            };

            return Ok(response);
        }
    }
}
