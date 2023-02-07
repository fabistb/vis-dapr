using Microsoft.AspNetCore.Mvc;

namespace Middleware.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/middleware")]
public class MiddlewareController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RateLimit([FromBody] object requestBody)
    {
        return new OkObjectResult(requestBody);
    }
}