using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Arma.Demo.Core;
public class ContextData
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