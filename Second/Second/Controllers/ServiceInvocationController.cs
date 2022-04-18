using Microsoft.AspNetCore.Mvc;
using Second.Models;

namespace Second.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{v:apiVersion}/service-invocation")]
public class ServiceInvocationController : ControllerBase
{
    private readonly ILogger<ServiceInvocationController> _logger;

    public ServiceInvocationController(ILogger<ServiceInvocationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] ServiceInvocationDto request)
    {
        _logger.LogInformation($"Receive message: {request.Message}");

        return Ok();
    }
}