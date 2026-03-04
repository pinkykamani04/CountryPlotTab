using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/kpi/otif")]
    public class CustomerOtifKpiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerOtifKpi(string customerId)
        {
            var kpis = new[]
            {
                new {
                    customerId = "CUS-1",
                    name = "OTIF KPI",
                    value = 92.5,
                    target = 95.0,
                    onTimePercentage = 90.2m,
                    inFullPercentage = 94.1m
                },
                new {
                    customerId = "CUS-2",
                    name = "OTIF KPI",
                    value = 88.0,
                    target = 92.0,
                    onTimePercentage = 85.5m,
                    inFullPercentage = 90.3m
                }
            };

            var result = kpis.FirstOrDefault(k =>
                k.customerId == customerId
            );

            if (result == null)
                return NotFound("OTIF KPI not found.");

            return Ok(result);
        }
    }
}
