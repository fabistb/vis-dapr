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

    public BindingController(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [HttpPost]
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



}