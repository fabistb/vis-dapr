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

    [HttpPost("pub-sub-send")]
    public async Task<IActionResult> PublishMessage([FromBody] PubSubMessageDto message)
    {
        await _pubSubService.PublishMessage("pub-sub-receive", message.Message);
        return Ok();
    }
    
    [Topic("messagebus", "pub-sub-receive")]
    [HttpPost("pub-sub-receive")]
    public async Task<IActionResult> ReceiveMessage([FromBody] PubSubMessageDto messageDto)
    {
        _logger.LogInformation($"PubSub message: {messageDto.Message}");
        return Ok();
    }
    
    [HttpPost("pub-sub-filter")]
    public async Task<IActionResult> PublishMessageFilter([FromBody] PubSubMessageDto message)
    {
        await _pubSubService.PublishMessage("pub-sub-filter", message.Message, message.Version);
        return Ok();
    }
    
    [Topic("messagebus", "pub-sub-filter", "event.data.version ==\"1\"", 1)]
    [HttpPost("version-1")]
    public async Task<IActionResult> ReceiveMessageVersion1([FromBody] PubSubMessageDto messageDto)
    {
        _logger.LogInformation($"PubSub filter message: {messageDto.Message}");
        return Ok();
    }
    
    [Topic("messagebus", "pub-sub-filter", "event.data.version ==\"2\"", 2)]
    [HttpPost("version-2")]
    public async Task<IActionResult> ReceiveMessageVersion2([FromBody] PubSubMessageDto messageDto)
    {
        _logger.LogInformation($"PubSub filter message: {messageDto.Message}");
        return Ok();
    }
}