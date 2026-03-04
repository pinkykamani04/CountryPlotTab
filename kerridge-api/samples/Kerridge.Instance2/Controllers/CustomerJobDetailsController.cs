using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customers/{customerId}/job/{jobNumber}")]
    public class CustomerJobDetailsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetJobDetails(string customerId, string jobNumber)
        {
            var jobs = new[]
            {
                new {
                    customerId = "CUS-3",
                    operativeName = "Gego",
                    operativeId = "Customer3",
                    jobNumber = "12234",
                    jobAddress = "9/20 xyx abc1",
                    txnLimit = "150",
                    dailyLimit = "150",
                    weeklyLimit = "150",
                    monthlyLimit = "150",
                    tradeCardNumber = "14564432",
                    spendRemainig = "23451",
                },
                new {
                    customerId = "CUS-4",
                    operativeName = "chego",
                    operativeId = "Customer4",
                    jobNumber = "12334",
                    jobAddress = "9/19 xyx abc2",
                    txnLimit = "350",
                    dailyLimit = "350",
                    weeklyLimit = "350",
                    monthlyLimit = "350",
                    tradeCardNumber = "131232323232",
                    spendRemainig = "2121",
                },
            };

            var result = jobs.FirstOrDefault(j =>
                j.customerId == customerId && j.jobNumber == jobNumber
            );

            if (result == null)
                return NotFound("Job not found.");

            return Ok(result);
        }
    }
}
