using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/orders")]
    public class CustomerOrdersController: ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerOrders(string customerId)
        {
            var orders = new[]
            {
                new {
                    id = "ORD-1001",
                    customerId = "CUS-1",
                    amount = 250.75,
                    status = "Completed",
                    operativeId = "OP-001",
                    operativeName = "John Doe",
                    orderDate = new DateTime(2025, 01, 10),
                    category = "Materials",
                    branch = "Milton Keynes",
                    transactionType = "Invoice"
                },
                new {
                    id = "ORD-1002",
                    customerId = "CUS-1",
                    amount = 480.30,
                    status = "In Progress",
                    operativeId = "OP-002",
                    operativeName = "Sarah Smith",
                    orderDate = new DateTime(2025, 01, 11),
                    category = "Materials",
                    branch = "Milton Keynes",
                    transactionType = "Invoice"
                },
                new {
                    id = "ORD-2001",
                    customerId = "CUS-2",
                    amount = 120.00,
                    status = "Completed",
                    operativeId = "OP-003",
                    operativeName = "Michael Lee",
                    orderDate = new DateTime(2025, 01, 15),
                    category = "Materials",
                    branch = "Milton Keynes",
                    transactionType = "Invoice"
                }
            };

            var result = orders
                .Where(o => o.customerId == customerId)
                .ToList();

            if (!result.Any())
                return NotFound("No orders found for this customer.");

            return Ok(result);
        }
    }
}
