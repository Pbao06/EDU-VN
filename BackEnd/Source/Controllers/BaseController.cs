using Microsoft.AspNetCore.Mvc;

namespace Source.Controllers
{
    public class BaseController : ControllerBase
    {
        [Route("api/[controller]")]
        protected IActionResult Success(object? data = null, string message = "Success")
        {
            return Ok(new
            {
                message,
                data
            });
        }
    }
}
