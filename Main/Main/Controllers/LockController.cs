using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Toolkit.HighPerformance.Helpers;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class LockController : ControllerBase
{
    private readonly ILockService _lockService;

    public LockController(ILockService lockService)
    {
        _lockService = lockService;
    }

    [HttpPost("lock")]
    public async Task<IActionResult> LockResource()
    {
        await _lockService.LockResource();
        
        return Ok();
    }

    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockResource()
    {
        await _lockService.UnlockResource();

        return Ok();
    }
}