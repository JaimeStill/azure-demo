using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Primary.Controllers;

struct SecretResult
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Message { get; set; }
}

[Route("api/[controller]")]
public class SecretController : Controller
{
    readonly IConfiguration config;

    public SecretController(IConfiguration config)
    {
        this.config = config;
    }

    IActionResult NoSecret(string name) => StatusCode(
        StatusCodes.Status500InternalServerError,
        $"Error: no secret named {name} was found..."
    );

    IActionResult Secret(string name, string value) => Json(
        new SecretResult()
        {
            Name = name,
            Value = value,
            Message = "This is for testing only! Never output a secret to a response or anywhere else in a real app!"
        }
    );

    [HttpGet("{secret?}")]
    public IActionResult Get([FromRoute]string secret = "DatabaseConnection")
    {
        string? value = config[secret];

        return value is null
            ? NoSecret(secret)
            : Secret(secret, value);
    }
}