using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuseumSystem.Application.Dtos.AIChatDtos;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using MuseumSystem.Domain.Options;

namespace MuseumSystem.Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GeminiService> _logger;
        private static string _basePrompt = "";

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings, IUnitOfWork unitOfWork, IMapper mapper, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            if (string.IsNullOrEmpty(_basePrompt))
            {
                var fullPath = Path.Combine(AppContext.BaseDirectory, "Utils", "Prompting.txt");
                _basePrompt = File.ReadAllText(fullPath);
            }
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
                if (isProcess)
                {
                    request.Headers.Add("x-goog-api-key", _settings.ApiKeyProcess);
                }
                else
                {
                    request.Headers.Add("x-goog-api-key", _settings.ApiKey);
                }
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

        public async Task<string> SendPrompting(string prompt, string museumId)
        {
            string baseDirectory = AppContext.BaseDirectory;

            //0. Read initial prompt template

            var fullPrompt = $"Message Customer: {prompt} \n {_basePrompt}";

            //1. Generate response from Gemini API - 1
            string response = await GenerateTextAsync(fullPrompt, false);

            if (string.IsNullOrWhiteSpace(response) || !response.TrimStart().StartsWith("{") || !response.Contains("\"action\""))
            {
                return response.Trim();
            }

            //2. Paraphrase the response
            ParapahraseResponse paraphrasedResponse = await ParapahraseText(response, museumId);

            //3. Send back the paraphrased response to  Gemini API - 2 for final output
            string promptPathFinal = Path.Combine(baseDirectory, "Utils", "PromptingFinal.txt");
            var startPromptFinal = await File.ReadAllTextAsync(promptPathFinal);

            string jsonData = JsonSerializer.Serialize(paraphrasedResponse.data);

            var fullPromptFinal = string.Concat(startPromptFinal, "\n", $"language: {paraphrasedResponse.language}", "\n", "Data: " + jsonData);

            //4. Get final response
            string finalResponse = await GenerateTextAsync(fullPromptFinal, true);
            return finalResponse.Replace("*", "");
        }

        public async Task<ParapahraseResponse> ParapahraseText(string text, string museumId)
        {
            JsonElement rawJson;
            ParapahraseResponse response = new ParapahraseResponse();

            try
            {
                using (JsonDocument document = JsonDocument.Parse(text))
                {
                    rawJson = document.RootElement.Clone();
                }
            }
            catch (JsonException ex)
            {
                throw new JsonException("Invalid JSON format", ex);
            }

            if (!rawJson.TryGetProperty("action", out JsonElement actionElement) ||
            actionElement.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("Missing or invalid 'action' property in JSON");
            }
            string action = actionElement.GetString()!;
            string language = rawJson.TryGetProperty("language", out JsonElement langElement) ? langElement.GetString()! : "vi";

            object? specificRequest = null;

            switch (action)
            {
                case "getArtifactList":
                    specificRequest = JsonSerializer.Deserialize<BaseResponse>(text);
                    var paginatedArtifacts = await GetAllArtifacts(
                        pageIndex: 1,
                        pageSize: 5,
                        museumId: museumId
                        );
                    response.language = language;
                    response.data = paginatedArtifacts;
                    return response;

                case "getMuseumInfo":
                    specificRequest = JsonSerializer.Deserialize<MuseumInfo>(text);
                    var museumResponse = await MuseumResponseV1(museumId);
                    response.language = language;
                    response.data = museumResponse;
                    break;

                case "getArtifactInfo":
                    {
                        string? artifactName = null;
                        string? artifactPeriod = null;
                        bool? isOrigin = null;
                        string? displayPosition = null;
                        string? area = null;

                        if (rawJson.TryGetProperty("artifactName", out JsonElement nameEl))
                            artifactName = nameEl.GetString();

                        if (rawJson.TryGetProperty("artifactPeriod", out JsonElement periodEl))
                            artifactPeriod = periodEl.GetString();

                        if (rawJson.TryGetProperty("isOrigin", out JsonElement originEl))
                            isOrigin = originEl.ValueKind == JsonValueKind.True ||
                                       (originEl.ValueKind == JsonValueKind.String &&
                                        originEl.GetString()?.ToLower() == "true");

                        if (rawJson.TryGetProperty("displayPosition", out JsonElement posEl))
                            displayPosition = posEl.GetString();

                        if (rawJson.TryGetProperty("area", out JsonElement areaEl))
                            area = areaEl.GetString();

                        var artifacts = await GetArtifactInfo(1, 5,
                            artifactName, artifactPeriod, isOrigin, displayPosition, area, museumId
                        );

                        response.language = language;
                        response.data = artifacts;
                        return response;
                    }
                case "getAreaInfo":
                    {
                        string? areaName = null;
                        specificRequest = JsonSerializer.Deserialize<ListAreasInfo>(text);
                        if (rawJson.TryGetProperty("areaName", out JsonElement areaEl))
                            areaName = areaEl.GetString();
                        var areas = await GetAreaInfo(1, 10, museumId);
                        response.language = language;
                        response.data = areas;
                        return response;
                    }

                case "getAreaList":
                    {
                        specificRequest = JsonSerializer.Deserialize<GetListAreasInfo>(text);
                        var areas = await GetAreaList(museumId);
                        response.language = language;
                        response.data = areas;
                        return response;
                    }

                case "getArtifactsInArea":
                    {
                        string? areaName = null;
                        specificRequest = JsonSerializer.Deserialize<ArtifactInfoInArea>(text);
                        if (rawJson.TryGetProperty("areaName", out JsonElement areaEl))
                            areaName = areaEl.GetString();
                        var artifactsInArea = await GetArtifactsInArea(1, 10, areaName, museumId);
                        response.language = language;
                        response.data = artifactsInArea;
                        return response;
                    }
                case "getExhibitions":
                    {
                        specificRequest = JsonSerializer.Deserialize<ExhibitionDetailsInfo>(text);
                        // Implement GetExhibitions method if needed
                        response.language = language;
                        response.data = "Chức năng đang trong quá trình phát triển";
                        return response;
                    }
                case "getExhibitionDetails":
                    {
                        specificRequest = JsonSerializer.Deserialize<ExhibitionDetailsInfo>(text);
                        // Implement GetExhibitionDetails method if needed
                        response.language = language;
                        response.data = "Chức năng đang trong quá trình phát triển";
                        return response;
                    }

                default:
                    return new ParapahraseResponse { language = language, data = "Chức năng không tồn trại hoặc đang trong quá trình phát triển" };
            }
            return response;
        }

        private async Task<BasePaginatedList<ArtifactResponse>> GetAllArtifacts(
            int pageIndex = 1,
            int pageSize = 5,
            string? museumId = null!)
        {
            var query = _unitOfWork.ArtifactRepository.Entity
              .Include(a => a.Museum)
              .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area)
              .Where(a => a.Status == ArtifactStatus.OnDisplay && a.MuseumId == museumId);


            query = query.OrderBy(a => a.Name);

            BasePaginatedList<Artifact> paginatedList = await _unitOfWork.ArtifactRepository.GetPagging(query, pageIndex, pageSize);

            var result = _mapper.Map<BasePaginatedList<Artifact>, BasePaginatedList<ArtifactResponse>>(paginatedList);
            return result;
        }

        private async Task<MuseumResponseV1> MuseumResponseV1(string museumId)
        {
            var museum = await _unitOfWork.MuseumRepository.Entity
                .FirstOrDefaultAsync(m => m.Id == museumId);
            if (museum == null)
            {
                return null;
            }
            var museumResponse = _mapper.Map<MuseumResponseV1>(museum);
            return museumResponse;
        }

        private async Task<BasePaginatedList<ArtifactResponse>> GetArtifactInfo(
            int pageIndex,
            int pageSize,
            string artifactName,
            string artifactPeriod,
            bool? isOrigin,
            string displayPosition,
            string area,
            string museumId)
        {
            var query = _unitOfWork.ArtifactRepository.Entity
              .Include(a => a.Museum)
              .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area)
              .Where(a => a.Status == ArtifactStatus.OnDisplay && a.MuseumId == museumId);

            if (!string.IsNullOrEmpty(artifactName))
            {
                query = query.Where(a => a.Name.ToLower().Contains(artifactName.ToLower()));
            }

            if (!string.IsNullOrEmpty(artifactPeriod))
            {
                query = query.Where(a => a.PeriodTime.ToLower().Contains(artifactPeriod.ToLower()));
            }

            if (isOrigin.HasValue)
            {
                query = query.Where(a => a.IsOriginal == isOrigin.Value);
            }

            if (!string.IsNullOrEmpty(displayPosition))
            {
                query = query.Where(a => a.DisplayPosition != null &&
                                         a.DisplayPosition.DisplayPositionName.ToLower().Contains(displayPosition.ToLower()));
            }

            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(a => a.DisplayPosition != null &&
                                         a.DisplayPosition.Area != null &&
                                         a.DisplayPosition.Area.Name.ToLower().Contains(area.ToLower()));
            }

            var artifact = await _unitOfWork.ArtifactRepository.GetPagging(query, pageIndex, pageSize);
            var result = _mapper.Map<BasePaginatedList<Artifact>, BasePaginatedList<ArtifactResponse>>(artifact);

            return result;
        }

        private async Task<BasePaginatedList<AreaResponse>> GetAreaList(string museumId)
        {
            var query = _unitOfWork.AreaRepository.Entity
              .Include(a => a.Museum)
              .Where(a => a.MuseumId == museumId);
            query = query.OrderBy(a => a.Name);
            BasePaginatedList<Area> paginatedList = await _unitOfWork.AreaRepository.GetPagging(query, 1, 10);
            var result = _mapper.Map<BasePaginatedList<Area>, BasePaginatedList<AreaResponse>>(paginatedList);
            return result;
        }

        private async Task<BasePaginatedList<AreaResponse>> GetAreaInfo(
            int pageIndex = 1,
            int pageSize = 5,
            string? museumId = null!,
            string? areaName = null!)
        {
            var query = _unitOfWork.AreaRepository.Entity
              .Include(a => a.Museum)
              .Where(a => a.MuseumId == museumId 
              && (string.IsNullOrEmpty(areaName) || a.Name.ToLower().Contains(areaName.ToLower())));

            query = query.OrderBy(a => a.Name);
            BasePaginatedList<Area> paginatedList = await _unitOfWork.AreaRepository.GetPagging(query, pageIndex, pageSize);
            var result = _mapper.Map<BasePaginatedList<Area>, BasePaginatedList<AreaResponse>>(paginatedList);
            return result;
        }


        private async Task<BasePaginatedList<ArtifactResponse>> GetArtifactsInArea(
            int pageIndex = 1,
            int pageSize = 5,
            string? areaName = null!,
            string? museumId = null!)
        {
            var query = _unitOfWork.ArtifactRepository.Entity
              .Include(a => a.Museum)
              .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area)
              .Where(a => a.Status == ArtifactStatus.OnDisplay && a.MuseumId == museumId);
            if (!string.IsNullOrEmpty(areaName))
            {
                query = query.Where(a => a.DisplayPosition != null &&
                                         a.DisplayPosition.Area != null &&
                                         a.DisplayPosition.Area.Name.ToLower().Contains(areaName.ToLower()));
            }
            query = query.OrderBy(a => a.Name);
            BasePaginatedList<Artifact> paginatedList = await _unitOfWork.ArtifactRepository.GetPagging(query, pageIndex, pageSize);
            var result = _mapper.Map<BasePaginatedList<Artifact>, BasePaginatedList<ArtifactResponse>>(paginatedList);
            return result;
        }
    }
}