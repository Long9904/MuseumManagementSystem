namespace MuseumSystem.Application.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateTextAsync(string prompt, bool isProcess);

        Task<string> SendPrompting(string prompt, string museumId);
    }
}
