using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // Cần cái này để có [ApiController] và [Route]
using Source.Controllers;
using Source.Controllers.Admin;
namespace Source.Controllers.Admin
{
    [Authorize(Roles="Admin")]// jwt token payload role == admin moi dc 
    [ApiController]
    [Route("api/admin/[controller]")]
    public class BaseAdminController: BaseController // tai su dung ke thua , da hinh 
    { 
        // doneeee
    }
}