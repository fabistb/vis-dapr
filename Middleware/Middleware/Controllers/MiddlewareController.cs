using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Middleware.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/middleware")]
public class MiddlewareController : ControllerBase
{
    [HttpPost]
    public ActionResult<JsonElement> RateLimit([FromBody] JsonElement body) => Ok(body);
}
