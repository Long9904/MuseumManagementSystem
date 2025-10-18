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

        Task<ArtifactResponse> GetArtifactById(
            string id,
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task<BasePaginatedList<ArtifactResponse>> GetAllArtifacts(
            int pageIndex,
            int pageSize,
            string? artifactCode,
            string? name,
            string? periodTime,
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task ActiveArtifact(
            string id,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> AssignArtifactToDisplayPosition(
            string artifactId,
            string displayPositionId,
            CancellationToken cancellationToken = default);

        Task<ArtifactResponse> RemoveArtifactFromDisplayPosition(
            string artifactId,
            CancellationToken cancellationToken = default);
    }
}
