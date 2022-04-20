using Main.Actors;
using Main.Infrastructure;
using Main.Models;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/actor")]
public class ActorController : ControllerBase
{
    private readonly IActorFactory<IMathActor> _mathActorFactory;

    private const string ActorId = "mathactor";

    public ActorController(IActorFactory<IMathActor> mathActorFactory)
    {
        _mathActorFactory = mathActorFactory;
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetActorState([FromBody] MathActorState state)
    {
        var actor = _mathActorFactory.CreateActor(ActorId);
        await actor.SetActorState(state);

        return Ok();
    }

    [HttpPost("addition")]
    public async Task<IActionResult> Addition([FromBody] MathOperationDto request)
    {
        var actor = _mathActorFactory.CreateActor(ActorId);
        var result = await actor.Addition(request.Value);
        
        return new OkObjectResult(result);
    }

    [HttpPost("subtraction")]
    public async Task<IActionResult> Substraction([FromBody] MathOperationDto request)
    {
        var actor = _mathActorFactory.CreateActor(ActorId);
        var result = await actor.Substraction(request.Value);

        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> ActorState()
    {
        var actor = _mathActorFactory.CreateActor(ActorId);
        var result = await actor.Get();

        return new OkObjectResult(result);
    }
}