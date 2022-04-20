using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/secrets")]
public class SecretsController : ControllerBase
{
    private readonly ISecretStoreService _secretStoreService;

    public SecretsController(ISecretStoreService secretStoreService)
    {
        _secretStoreService = secretStoreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSecretValue([FromQuery] string secretName)
    {
        var result = await _secretStoreService.GetSecretBySecretName(secretName);
        
        return new OkObjectResult(result);
    }

    [HttpGet("bulk")]
    public async Task<IActionResult> GetSecrets()
    {
        var result = await _secretStoreService.GetSecrets();

        return new OkObjectResult(result);
    }
    
    
}