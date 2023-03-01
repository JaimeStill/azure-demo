using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace Arma.Demo.Core.Middleware;

public class JsonExceptionMiddleware
{
    readonly RequestDelegate next;

    public JsonExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration config, IWebHostEnvironment env)
    {
        await next(context);

        IExceptionHandlerFeature error = context.Features.Get<IExceptionHandlerFeature>();

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = Application.Json;

        JsonExceptionData response = new(context, error);
        await context.Response.WriteAsJsonAsync(response);
        await response.LogError(InitializeLogPath(config, env));
    }

    static string InitializeLogPath(IConfiguration config, IWebHostEnvironment env)
    {
        string path = config.GetValue<string>("JsonExceptionLogPath")
            ?? Path.Join(env.WebRootPath, "exceptions");

        if (!Path.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
}

public static class JsonExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseJsonExceptionHandler(this IApplicationBuilder app) =>
        app.UseExceptionHandler(errorApp =>
            errorApp.UseMiddleware<JsonExceptionMiddleware>()
        );
}