using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/resiliency")]
public class ResiliencyController : ControllerBase
{
    private readonly IResiliencyService _resiliencyService;

    public ResiliencyController(IResiliencyService resiliencyService)
    {
        _resiliencyService = resiliencyService;
    }

    [HttpPost]
    public async Task<IActionResult> TriggerResiliency()
    {
        await _resiliencyService.ExhaustResource();

        return Ok();
    }
}