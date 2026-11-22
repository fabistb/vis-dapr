using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Second.Models;

namespace Second.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/service-invocation")]
public class ServiceInvocationController : ControllerBase
{
    private readonly ILogger<ServiceInvocationController> _logger;

    public ServiceInvocationController(ILogger<ServiceInvocationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Receive([FromBody] ServiceInvocationDto request)
    {
        _logger.LogInformation("Receive message: {Message}", request.Message);

        return Ok();
    }
}