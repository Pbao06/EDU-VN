using Microsoft.AspNetCore.Mvc;

namespace Source.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
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
