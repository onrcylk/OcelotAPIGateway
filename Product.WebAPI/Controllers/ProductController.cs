using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Product.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        [HttpGet("GetProducts")]
        public IActionResult Get()
        {
            return Ok(new { Data="This is Product Service" });
        }
    }
}