using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/conversation")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }
    
    [HttpPost("ask-llm")]
    public async Task<IActionResult> AskLLM([FromBody] ConversationDto conversationDto)
    {
        var answer = await _conversationService.Conversation(conversationDto);
        
        return new OkObjectResult(answer);
    }
}