using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace Core.Controllers;

class ContextData
{
    public string ContentType { get; private set; }
    public string Url { get; private set; }
    public string User { get; private set; }
    public string UserId { get; private set; }

    public ContextData(HttpContext context)
    {
        ContentType = context.Request.ContentType ?? "N/A";
        Url = context.Request.GetDisplayUrl();
        User = context.User?.GetDisplayName() ?? "N/A";
        UserId = context.User?.GetObjectId() ?? "N/A";
    }
}

[Authorize]
[RequiredScope(RequiredScopesConfigurationKey = "Scopes")]
[Route("api/[controller]")]
public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        Json(new ContextData(HttpContext));
}