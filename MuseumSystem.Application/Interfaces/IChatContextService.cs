using MuseumSystem.Application.Dtos.AIChatDtos;

namespace MuseumSystem.Application.Interfaces
{
    public interface IChatContextService
    {
        Task<ChatContext> GetContextAsync(string userId);
        Task SetContextAsync(string userId, ChatContext context);
        Task ClearContextAsync(string userId);
    }
}
