using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/categories-by-spend")]
    public class CategoryBySpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBranchesBySpend()
        {
            var operatives = new[]
            {
                new
                {
                    CategoryName = "Category 1",
                    CategorySpend = "12,500"
                },
                new
                {
                    CategoryName = "Category 2",
                    CategorySpend = "9,800"
                },
                new
                {
                    CategoryName = "Category 3",
                    CategorySpend = "7,200"
                }
            };

            return Ok(operatives);
        }
    }
}
