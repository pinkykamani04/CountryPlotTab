using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
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
                "CUS-1",
                "CUS-2"
            };

            if (!customers.Contains(customerId))
                return NotFound("Customer not found.");

            var response = new
            {
                customerId = customerId,
                newSpendLimit = newLimit
            };

            return Ok(response);
        }
    }
}
