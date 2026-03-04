using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance3.Controllers
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
                "CUS-5",
                "CUS-6"
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
