using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerDiplom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GreetingController : ControllerBase
    {
        // GET: api/greeting
        [HttpGet]
        public IActionResult GetGreeting()
        {
            return Ok("Привет от сервера!");
        }
    }
}
