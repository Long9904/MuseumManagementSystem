
namespace MuseumSystem.Application.Dtos.AIChatDtos
{
    public class PromptRequest
    {
        public string Prompt { get; set; } = string.Empty;

        public string MuseumId { get; set; } = string.Empty;
    }
}
