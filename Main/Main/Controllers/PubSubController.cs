using Dapr;
using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[Route("api/v1.0/pub-sub")]
public class PubSubController : ControllerBase
{
    private readonly IPubSubService _pubSubService;
    private readonly ILogger<PubSubController> _logger;

    public PubSubController(IPubSubService pubSubService, ILogger<PubSubController> logger)
    {
        _pubSubService = pubSubService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PublishMessage([FromBody] PubSubMessageDto message)
    {
        await _pubSubService.PublishMessage(message.Message);
        return Ok();
    }

    [Topic("messagebus", "pub-sub-receive")]
    [HttpPost("pub-sub-receive")]
    public async Task<IActionResult> ReceiveMessage([FromBody] PubSubMessageDto messageDto)
    {
        _logger.LogInformation($"PubSub message: {messageDto.Message}");
        return Ok();
    }
}