using MuseumSystem.Application.Dtos.ArtifactDtos;

namespace MuseumSystem.Application.Interfaces
{
    public interface IArtifactMediaService
    {
        Task<MediaResponse> UploadArtifactMediaAsync(
            string artifactId,
            MediaRequest mediaRequest);

        Task<MediaResponse> UpdateArtifactMediaAsync(
            string artifactId,
            string mediaId,
            MediaRequest mediaRequest);

        Task<bool> DeleteArtifactMediaAsync(
            string artifactId,
            string mediaId);

    }
}
