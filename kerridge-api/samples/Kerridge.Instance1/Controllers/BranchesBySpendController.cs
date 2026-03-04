using Microsoft.AspNetCore.Mvc;

namespace Kerridge.Instance1.Controllers
{
    [ApiController]
    [Route("api/v1/customer/branches-by-spend")]
    public class BranchesBySpendController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBranchesBySpend()
        {
            var operatives = new[]
            {
                new
                {
                    BranchName = "Branch 1",
                    BranchSpend = "12,500"
                },
                new
                {
                    BranchName = "Branch 2",
                    BranchSpend = "9,800"
                },
                new
                {
                    BranchName = "Branch 3",
                    BranchSpend = "7,200"
                }
            };

            return Ok(operatives);
        }
    }
}
