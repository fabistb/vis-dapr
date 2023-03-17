using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

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
        var result = await _stateStoreService.Update(request);

        if (result)
        {
            return Ok();
        }

        return new BadRequestObjectResult("eTag mismatch");
    }

    [HttpGet("filter-by-name")]
    public async Task<IActionResult> FilterByName()
    {
        var result = await _stateStoreService.FilterByFirstName();

        return new OkObjectResult(result);
    }

    [HttpGet("sort-by-age")]
    public async Task<IActionResult> SortByName()
    {
        var result = await _stateStoreService.SortByAge();

        return new OkObjectResult(result);
    }

    [HttpGet("page")]
    public async Task<IActionResult> Page()
    {
        var result = await _stateStoreService.Page();

        return new OkObjectResult(result);
    }

    [HttpPost("set-shared-state")]
    public async Task<IActionResult> SetSharedState([FromBody] StateStoreDto request)
    {
        await _stateStoreService.SetSharedState(request);

        return Ok();
    }
}