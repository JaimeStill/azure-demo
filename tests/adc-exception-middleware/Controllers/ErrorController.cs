using Microsoft.AspNetCore.Mvc;

namespace AdcMiddlewareTest.Controllers;

[Route("api/[controller]")]
public class ErrorController : Controller
{
    [HttpGet("{error?}")]
    public IActionResult Get([FromRoute]string error = "Something catastropic occurred. But only for demonstration purposes!") =>
        throw new Exception(error);
}