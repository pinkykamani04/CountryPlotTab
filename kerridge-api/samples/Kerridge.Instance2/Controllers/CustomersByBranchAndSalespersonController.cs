using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customers/by-branch/{branchCode}/salesperson/{salespersonCode}")]
    public class CustomersByBranchAndSalespersonController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomersByBranchAndSalesperson(string branchCode, string salespersonCode)
        {
            var customers = new[]
            {
                new {
                    id = "CUS-4",
                    name = "Acme Corporation",
                    accountStatus = 1,
                    creditLimit = 50000.0,
                    branchCode = "MAN02",
                    salespersonCode = "SP-01"
                },
                new {
                    id = "CUS-5",
                    name = "Global Industries",
                    accountStatus = 1,
                    creditLimit = 75000.0,
                    branchCode = "MAN02",
                    salespersonCode = "SP-02"
                },
                new {
                    id = "CUS-6",
                    name = "Tech Solutions",
                    accountStatus = 0,
                    creditLimit = 30000.0,
                    branchCode = "MAN02",
                    salespersonCode = "SP-01"
                }
            };

            var result = customers
                .Where(c => c.branchCode == branchCode && c.salespersonCode == salespersonCode)
                .ToList();

            if (!result.Any())
                return NotFound("No customers found for the given branch and salesperson.");

            return Ok(result);
        }
    }
}
