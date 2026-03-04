using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance2.Controllers
{
    [ApiController]
    [Route("api/v1/customer/operatives-by-spend")]
    public class OperativesBySpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetOperativesBySpend()
        {
            var operatives = new[]
            {
                new
                {
                    OperativeName = "John Doe",
                    OperativeSpend = "£12,500"
                },
                new
                {
                    OperativeName = "Sarah Smith",
                    OperativeSpend = "£9,800"
                },
                new
                {
                    OperativeName = "Michael Lee",
                    OperativeSpend = "£7,200"
                }
            };

            return Ok(operatives);
        }
    }
}
