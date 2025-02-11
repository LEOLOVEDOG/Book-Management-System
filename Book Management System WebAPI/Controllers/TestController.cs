using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Management_System_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("profile")]
        public ActionResult<string> Profile()
        {
            // 從 jwt 取得使用者名稱
            var userName = User.Identity?.Name;
            return Ok(userName);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok("Welcome, Admin!");
        }



    }
}
