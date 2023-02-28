using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DevCode.Controllers;

class ContextData
{
    public string ContentType { get; private set; }
    public string Url { get; private set; }
    public string User { get; private set; }

    public ContextData(HttpContext context)
    {
        ContentType = context.Request.ContentType ?? "N/A";
        Url = context.Request.GetDisplayUrl();
        User = context.User?.Identity?.Name ?? "N/A";
    }
}

[Authorize]
[Route("api/[controller]")]
public class AuthController : Controller
{
    static readonly string[] scopes = { "app_read" };

    [HttpGet]
    public IActionResult Get()
    {
        HttpContext.VerifyUserHasAnyAcceptedScope(scopes);

        return Json(new ContextData(HttpContext));
    }
}