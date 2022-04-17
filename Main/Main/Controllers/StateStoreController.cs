using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{v:apiVersion}/state-store")]
public class StateStoreController : ControllerBase
{
    private readonly IStateStoreService _stateStoreService;

    public StateStoreController(IStateStoreService stateStoreService)
    {
        _stateStoreService = stateStoreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetState()
    {
        var result = await _stateStoreService.Get();
        return new OkObjectResult(result);
    }

    [HttpGet("etag")]
    public async Task<IActionResult> GetStateEtag()
    {
        var (stateStoreDto, eTag) = await _stateStoreService.GetEtag();

        var returnObject = new
        {
            eTag,
            value = stateStoreDto,
        };

        return new OkObjectResult(returnObject);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteState()
    {
        await _stateStoreService.Delete();
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> SetState([FromBody] StateStoreDto request)
    {
        await _stateStoreService.Set(request);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateState([FromBody] StateStoreEtagDto request)
    {
        await _stateStoreService.Update(request);
        return Ok();
    }
}