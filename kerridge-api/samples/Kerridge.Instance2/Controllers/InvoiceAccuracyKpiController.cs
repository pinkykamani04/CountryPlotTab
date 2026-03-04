using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/kpi/invoice-accuracy")]
    public class InvoiceAccuracyKpiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoiceAccuracyKpi(string customerId)
        {
            var kpis = new[]
            {
                new {
                    customerId = "CUS-3",
                    name = "Invoice Accuracy",
                    value = 98.5,
                    target = 99.0,
                    totalInvoices = 120,
                    correctInvoices = 118,
                    accuracyPercentage = 98.33m
                },
                new {
                    customerId = "CUS-4",
                    name = "Invoice Accuracy",
                    value = 95.2,
                    target = 97.0,
                    totalInvoices = 90,
                    correctInvoices = 86,
                    accuracyPercentage = 95.55m
                }
            };

            var result = kpis.FirstOrDefault(k =>
                k.customerId == customerId
            );

            if (result == null)
                return NotFound("Invoice accuracy KPI not found.");

            return Ok(result);
        }
    }
}
