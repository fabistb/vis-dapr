using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/service-invocation")]
public class ServiceInvocationController : ControllerBase
{
    private readonly IServiceInvocationService _serviceInvocationService;

    public ServiceInvocationController(IServiceInvocationService serviceInvocationService)
    {
        _serviceInvocationService = serviceInvocationService;
    }

    [HttpPost]
    public async Task<IActionResult> InvokeService([FromBody] ServiceInvocationDto request)
    {
        await _serviceInvocationService.Invoke(request);
        return Ok();
    }
}