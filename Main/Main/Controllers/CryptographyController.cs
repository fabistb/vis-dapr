using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/cryptography")]
public class CryptographyController : ControllerBase
{
    private readonly ICryptographyService _cryptographyService;

    public CryptographyController(ICryptographyService cryptographyService)
    {
        _cryptographyService = cryptographyService;
    }

    [HttpPost("encrypt")]
    public async Task<IActionResult> Encrypt([FromBody] CryptographyDto cryptographyDto)
    {
        var encryptedMessage = await _cryptographyService.Encrypt(cryptographyDto);

        return new OkObjectResult(encryptedMessage);
    }

    [HttpPost("decrypt")]
    public async Task<IActionResult> Decrypt([FromBody] CryptographyDto cryptographyDto)
    {
        var decryptedMessage = await _cryptographyService.Decrypt(cryptographyDto);
        
        return new OkObjectResult(decryptedMessage);
    }
}
