using Arma.Demo.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Processor.Controllers;

[Route("api/[controller]")]
public class PingController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        Console.WriteLine("Ping: Pong");

        return Json(
            new ApiResult<string>(HttpContext.Request.GetDisplayUrl())
        );
    }
}