using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Application.Interfaces
{
    public interface IVisitorService
    {
        Task<VisitorResponse> RegisterVisitorAsync(VisitorRequest visitorRequest);

        Task<VisitorLoginResponse> LoginVisitorAsync(VisitorRequest visitorRequest);

        Task<VisitorResponse> MyProfileAsync();

        // Post interactions like comments, ratings, etc. can be added here in the future
        Task<MyInteractionResponse> PostInteractionAsync(InteractionRequest request);

        // My interactions
        Task<BasePaginatedList<MyInteractionResponse>> MyInteractionsAsync(int pageIndex, int pageSize);

        // Museum service
        Task<BasePaginatedList<MuseumResponseV1>> GetMuseumsAsync
            (int pageIndex, int pageSize, string? museumName = null);

        Task<MuseumResponseV1> GetMuseumByIdAsync(string museumId);

        // Artifact service

        Task<BasePaginatedList<ArtifactDetailsResponse>> GetAllArtifactsByMuseumAsync(
            string museumId, 
            int pageIndex, 
            int pageSize,
            string? artifactName = null,
            string? periodTime = null,
            string? areaName = null,
            string? displayPositionName = null);

        Task<ArtifactDetailsResponse> GetArtifactByIdAsync(string artifactId);

        Task<BasePaginatedList<VisitorInteractionResponse>> GetAllInteractionsByArtifactAsync(
            string artifactId,
            int pageIndex,
            int pageSize);
    }
}
