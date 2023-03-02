using Microsoft.AspNetCore.Mvc;
using Processor.Services;

[Route("api/[controller]")]
public class PingController : Controller
{
    readonly PingService ping;

    public PingController(PingService ping)
    {
        this.ping = ping;
    }

    [HttpGet("{key:guid}")]
    public async Task<IActionResult> Get([FromRoute]Guid key) =>
        Ok(await ping.Pong(key));
}