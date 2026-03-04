using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
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
                    customerId = "b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5",
                    operativeName = "Geko",
                    operativeId = "Customer1",
                    jobNumber = "12234",
                    jobAddress = "9/20 xyx abc1",
                    txnLimit = 150.00m,
                    dailyLimit = 150.00m,
                    weeklyLimit = 150.00m,
                    monthlyLimit = 150.00m,
                    tradeCardNumber = "14564432",
                    spendRemainig = 23451.00m,
                    status = "Active",
                },
                new {
                    customerId = "a77c955e-a64f-4d69-ab0f-3a9fb23efdc2",
                    operativeName = "Heko",
                    operativeId = "Customer2",
                    jobNumber = "12334",
                    jobAddress = "9/19 xyx abc2",
                    txnLimit = 350.00m,
                    dailyLimit = 350.00m,
                    weeklyLimit = 350.00m,
                    monthlyLimit = 350.00m,
                    tradeCardNumber = "131232323232",
                    spendRemainig = 2121.00m,
                    status = "Deactive",
                },
            };

            var result = jobs.FirstOrDefault(j => j.customerId == customerId && (j.jobNumber.ToLower() == jobNumber.ToLower() || j.operativeId.ToLower() == jobNumber.ToLower()));

            if (result == null)
                return NotFound("Job not found.");

            return Ok(result);
        }
    }
}
