using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}")]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomer(string customerId)
        {
            var customers = new[]
            {
                new
                {
                    customerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
                    accountNumbers = new[] { "AC-1001", "AC-1002" },
                    name = "John Doe",
                    accountStatus = 1,
                    creditLimit = 50000
                },
                new
                {
                    customerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    accountNumbers = new[] { "GI-2001" },
                    name = "Sarah Smith",
                    accountStatus = 1,
                    creditLimit = 75000
                }
            };

            var result = customers.FirstOrDefault(j =>
                j.customerId.ToString().Equals(customerId, StringComparison.OrdinalIgnoreCase)
            );

            if (result == null)
                return NotFound("Customer not found.");

            return Ok(result);
        }
    }
}
