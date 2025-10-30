namespace MuseumSystem.Application.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateTextAsync(string prompt, bool isProcess);
    }
}
