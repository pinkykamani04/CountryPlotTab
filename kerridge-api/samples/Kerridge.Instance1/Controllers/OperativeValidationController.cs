using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
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
                    customerId = "b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5",
                    operativeId = "Customer1",
                    firstName = "John",
                    surname = "Doe",
                    valid = 1
                },
                new {
                    customerId = "b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5",
                    operativeId = "OP-2",
                    firstName = "Emma",
                    surname = "Stone",
                    valid = 1
                },
                new {
                    customerId = "a77c955e-a64f-4d69-ab0f-3a9fb23efdc2",
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
