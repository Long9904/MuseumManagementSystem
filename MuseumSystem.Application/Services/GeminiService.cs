using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Options;

namespace MuseumSystem.Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GenerateTextAsync(string prompt, bool isProcess)
        {
            try
            {
                string endpoint = $"{_settings.BaseUrl}{_settings.Model}:generateContent";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("x-goog-api-key", _settings.ApiKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                string? text = null;

                if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        text = parts[0].GetProperty("text").GetString();
                    }
                }

                return text ?? "(No content)";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating text from Gemini API", ex);
            }
        }
    }
}
