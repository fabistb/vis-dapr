using Dapr.AI.Conversation;
using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class ConversationService : IConversationService
{
    private readonly DaprConversationClient _conversationClient;

    public ConversationService(DaprConversationClient conversationClient)
    {
        _conversationClient = conversationClient;
    }
    
    public async Task<string> Conversation(ConversationDto conversation)
    {
        var response = await _conversationClient.ConverseAsync("conversation",
            new List<DaprConversationInput>
            {
                new DaprConversationInput(
                    conversation.Prompt,
                    DaprConversationRole.Generic)
            });

        var output = response.Outputs.Aggregate(string.Empty, (current, resp) => current + resp.Result);
        return output;
    }
}