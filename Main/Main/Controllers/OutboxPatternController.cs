using CommunityToolkit.HighPerformance.Helpers;
using Dapr;
using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[Route("api/v1.0/outbox")]
public class OutboxPatternController : ControllerBase
{
    private readonly IOutboxPatternService _outboxPatternService;
    private readonly ILogger<OutboxPatternController> _logger;

    public OutboxPatternController(IOutboxPatternService outboxPatternService, ILogger<OutboxPatternController> logger)
    {
        _outboxPatternService = outboxPatternService;
        _logger = logger;
    }

    [HttpPost("change-state")]
    public async Task<IActionResult> ChangeState([FromBody] OutboxDto request)
    {
        await _outboxPatternService.ChangeState(request);
        return Ok();
    }

    [HttpPost("outbox-topic")]
    [Topic("messagebus", "outbox-topic")]
    public async Task<IActionResult> ProcessStateInformation([FromBody] object message)
    {
        _logger.LogInformation(message.ToString());
        return Ok();
    }
}