using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customers/{customerId}/operative/{operativeId}")]
    public class OperativeValidationController : ControllerBase
    {
        [HttpGet]
        public IActionResult ValidateOperative(string customerId, string operativeId)
        {
            var operatives = new[]
            {
                new {
                    customerId = "CUS-3",
                    operativeId = "OP-1",
                    firstName = "John",
                    surname = "Doe",
                    valid = 1
                },
                new {
                    customerId = "CUS-3",
                    operativeId = "OP-2",
                    firstName = "Emma",
                    surname = "Stone",
                    valid = 1
                },
                new {
                    customerId = "CUS-4",
                    operativeId = "OP-9",
                    firstName = "Invalid",
                    surname = "User",
                    valid = 0
                }
            };

            var result = operatives.FirstOrDefault(o =>
                o.customerId == customerId && o.operativeId == operativeId
            );

            if (result == null)
                return NotFound("Not found.");

            return Ok(result);
        }
    }
}
