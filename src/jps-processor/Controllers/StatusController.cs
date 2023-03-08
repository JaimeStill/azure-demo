using Arma.Demo.Core;
using Microsoft.AspNetCore.Mvc;
using Processor.Services;

namespace Processor.Controllers;

[Route("api/[controller]")]
public class StatusController : Controller
{
    readonly ProcessorService svc;

    public StatusController(ProcessorService svc)
    {
        this.svc = svc;
    }

    [HttpGet]
    public IActionResult Get() =>
        Json(
            new ApiResult<ProcessorStatus>(svc.GetStatus())
        );
}