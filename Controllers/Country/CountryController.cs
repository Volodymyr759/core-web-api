using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    public class CountryController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
