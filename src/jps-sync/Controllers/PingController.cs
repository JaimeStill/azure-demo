using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Sync.Controllers;

[Route("api/[controller]")]
public class PingController : Controller
{
    public IActionResult Get() =>
        Ok(HttpContext.Request.GetDisplayUrl());
}