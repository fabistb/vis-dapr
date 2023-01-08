using Microsoft.AspNetCore.Mvc;
using Second.Services;

namespace Second.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/state-store")]
public class StateStoreController : ControllerBase
{
    private readonly IStateStoreService _stateStoreService;

    public StateStoreController(IStateStoreService stateStoreService)
    {
        _stateStoreService = stateStoreService;
    }

    [HttpGet("shared-state-store")]
    public async Task<IActionResult> GetSharedStateStore()
    {
        var result = await _stateStoreService.GetSharedState();

        return new OkObjectResult(result);
    }
}