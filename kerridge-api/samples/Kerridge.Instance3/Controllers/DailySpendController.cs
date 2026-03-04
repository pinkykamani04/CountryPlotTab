using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance3.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/spend/day")]
    public class DailySpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTotalDaySpend(string customerId)
        {
            var spends = new[]
            {
                new {
                    customerId = "CUS-5",
                    totalSpendForDay = 350.75m
                },
                new {
                    customerId = "CUS-6",
                    totalSpendForDay = 120.40m
                }
            };

            var result = spends.FirstOrDefault(s =>
                s.customerId == customerId
            );

            if (result == null)
                return NotFound("Daily spend not found.");

            return Ok(result);
        }
    }
}
