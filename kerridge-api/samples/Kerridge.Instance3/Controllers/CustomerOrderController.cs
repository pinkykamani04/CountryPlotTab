using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance3.Controllers
{
    [ApiController]
    [Route("api/v1/customer/{customerId}/order/{orderId}")]
    public class CustomerOrderController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomerOrder(string customerId, string orderId)
        {
            var orders = new[]
            {
                new {
                    id = "ORD-1001",
                    customerId = "CUS-5",
                    amount = 250.75,
                    status = "Completed",
                    operativeId = "OP-001",
                    operativeName = "John Doe",
                    orderDate = new DateTime(2025, 01, 10),
                    isOnTime = true,
                    isInFull = true,
                    hasFaultyGoods = false
                },
                new {
                    id = "ORD-1002",
                    customerId = "CUS-5",
                    amount = 480.30,
                    status = "In Progress",
                    operativeId = "OP-002",
                    operativeName = "Sarah Smith",
                    orderDate = new DateTime(2025, 01, 11),
                    isOnTime = false,
                    isInFull = true,
                    hasFaultyGoods = false
                },
                new {
                    id = "ORD-2001",
                    customerId = "CUS-6",
                    amount = 120.00,
                    status = "Completed",
                    operativeId = "OP-003",
                    operativeName = "Michael Lee",
                    orderDate = new DateTime(2025, 01, 15),
                    isOnTime = true,
                    isInFull = false,
                    hasFaultyGoods = true
                }
            };

            var order = orders.FirstOrDefault(o =>
                o.customerId == customerId &&
                o.id == orderId
            );

            if (order == null)
                return NotFound("Order not found.");

            return Ok(order);
        }
    }
}
