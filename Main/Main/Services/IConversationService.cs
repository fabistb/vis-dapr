using Main.Models;

namespace Main.Services;

public interface IConversationService
{
    Task<string> Conversation(ConversationDto conversation);
}