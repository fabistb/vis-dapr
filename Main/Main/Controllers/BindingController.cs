using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/binding")]
public class BindingController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    
    private readonly IServiceBusQueueService _serviceBusQueueService;
    private readonly ILogger<BindingController> _logger;

    public BindingController(
        IBlobStorageService blobStorageService,
        IServiceBusQueueService serviceBusQueueService,
        ILogger<BindingController> logger)
    {
        _blobStorageService = blobStorageService;
        _serviceBusQueueService = serviceBusQueueService;
        _logger = logger;
    }

    [HttpPost("blob")]
    public async Task<IActionResult> CreateBlob([FromBody] CreateBlobDto request)
    {
        var blobUrl = await _blobStorageService.Create(request);

        return new OkObjectResult(blobUrl);
    }

    [HttpGet("blob")]
    public async Task<IActionResult> GetBlob([FromQuery] string blobName, bool metadata = false)
    {
        var result = await _blobStorageService.Get(blobName, metadata);

        return new OkObjectResult(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListBlobs([FromBody] BlobStorageListDto request)
    {
        var result = await _blobStorageService.List(request);

        return new OkObjectResult(result);
    }

    [HttpDelete("{blobName}")]
    public async Task<IActionResult> DeleteBlob([FromRoute] string blobName)
    {
        await _blobStorageService.Delete(blobName);

        return Ok();
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendServiceBusMessage([FromBody] ServiceBusMessageDto message)
    {
        await _serviceBusQueueService.AddMessage(message);

        return Ok();
    }

    [HttpPost]
    [Route("/servicebusqueue")]
    public async Task<IActionResult> ReceiveServiceBusMessage([FromBody] ServiceBusMessageDto message)
    {
        _logger.LogInformation("ServiceBus Queue message: {message}", message.Message);

        return Ok();
    }
}