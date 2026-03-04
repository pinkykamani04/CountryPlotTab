using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/products-by-spend")]
    public class ProductsBySpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBranchesBySpend()
        {
            var operatives = new[]
            {
                new
                {
                    ProductName = "Product 1",
                    ProductSpend = "12,500"
                },
                new
                {
                    ProductName = "Product 2",
                    ProductSpend = "9,800"
                },
                new
                {
                    ProductName = "Product 3",
                    ProductSpend = "7,200"
                }
            };

            return Ok(operatives);
        }
    }
}
