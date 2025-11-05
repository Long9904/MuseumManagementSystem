using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Application.Interfaces
{
    public interface IArtifactService
    {
        Task<ArtifactResponse> CreateArtifact(
            ArtifactRequest request,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> UpdateArtifact(
            string id,
            ArtifactRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteArtifact(
            string id,
            CancellationToken cancellationToken = default);

        Task<ArtifactDetailsResponse> GetArtifactById(
            string id,
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task<BasePaginatedList<ArtifactResponse>> GetAllArtifacts(
            int pageIndex,
            int pageSize,
            string? name,
            string? periodTime,
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task MaintainaceArtifact(
            string id,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> AssignArtifactToDisplayPosition(
            string artifactId,
            string displayPositionId,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> RemoveArtifactFromDisplayPosition(
            string artifactId,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> GetArtifactByCode(
            string code,
            CancellationToken cancellationToken = default);
    }
}
