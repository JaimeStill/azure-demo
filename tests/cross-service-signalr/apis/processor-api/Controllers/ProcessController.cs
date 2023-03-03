using Arma.Demo.Core.Processing;
using Microsoft.AspNetCore.Mvc;
using Processor.Services;

namespace Processor.Controllers;

[Route("api/[controller]")]
public class ProcessController : Controller
{
    readonly ProcessorService processor;

    public ProcessController(ProcessorService processor)
    {
        this.processor = processor;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]Package package)
    {
        Console.WriteLine($"Received package {package.Name} with key {package.Key}");
        bool result = await processor.ProcessPackage(package);
        return Ok(result);
    }
}