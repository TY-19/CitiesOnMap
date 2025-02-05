using Microsoft.AspNetCore.Mvc;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet("~/")]
    public ActionResult GoToSwagger()
    {
        return Redirect("/swagger/index.html");
    }
}
