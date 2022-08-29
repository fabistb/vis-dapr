using Microsoft.AspNetCore.Mvc;
using Second.Actors;
using Second.Infastructure;

namespace Second.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/resiliency")]
public class ResiliencyController : ControllerBase
{
    private const string ActorId = "resiliency";
    
    private readonly IActorFactory<IResiliencyActor> _actorFactory;

    public ResiliencyController(IActorFactory<IResiliencyActor> actorFactory)
    {
        _actorFactory = actorFactory;
    }

    [HttpPost]
    public async Task<IActionResult> ExhauseResource()
    {
        var actor = _actorFactory.CreateActor(ActorId);
        await actor.SimulateExhaustedResource();

        return Ok();
    }
}