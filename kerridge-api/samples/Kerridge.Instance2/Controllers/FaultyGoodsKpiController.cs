using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/kpi/faulty-goods")]
    public class FaultyGoodsKpiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFaultyGoodsKpi(string customerId)
        {
            var kpis = new[]
            {
                new {
                    customerId = "CUS-3",
                    name = "Faulty Goods KPI",
                    value = 2.5,
                    target = 1.0,
                    totalOrders = 200,
                    faultyOrders = 5,
                    faultyPercentage = 2.5m
                },
                new {
                    customerId = "CUS-4",
                    name = "Faulty Goods KPI",
                    value = 1.2,
                    target = 1.0,
                    totalOrders = 150,
                    faultyOrders = 2,
                    faultyPercentage = 1.33m
                }
            };

            var result = kpis.FirstOrDefault(k =>
                k.customerId == customerId
            );

            if (result == null)
                return NotFound("Faulty goods KPI not found.");

            return Ok(result);
        }
    }
}
