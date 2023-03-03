using Processor.Services;

namespace Processor.Middleware;
public class ProcessorMiddleware
{
    readonly RequestDelegate next;

    public ProcessorMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, ProcessorService processor)
    {
        await processor.Register();
        await next(context);
    }
}

public static class ProcessorMiddlewareExtensions
{
    public static IApplicationBuilder UseProcessor(this IApplicationBuilder app) =>
        app.UseMiddleware<ProcessorMiddleware>();
}