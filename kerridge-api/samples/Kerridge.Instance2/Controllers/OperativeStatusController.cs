using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/operative/status/{operativeStatus}")]
    public class OperativeStatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetOperativeStatus(string customerId, int operativeStatus)
        {
            var statuses = new[]
            {
                new {
                    customerId = "CUS-3",
                    operativeStatus = 1
                },
                new {
                    customerId = "CUS-3",
                    operativeStatus = 0
                },
                new {
                    customerId = "CUS-4",
                    operativeStatus = 1
                },
                new {
                    customerId = "CUS-4",
                    operativeStatus = 0
                }
            };

            var result = statuses.FirstOrDefault(s =>
                s.customerId == customerId &&
                s.operativeStatus == operativeStatus
            );

            if (result == null)
                return NotFound("Operative status not found.");

            return Ok(result);
        }
    }
}
