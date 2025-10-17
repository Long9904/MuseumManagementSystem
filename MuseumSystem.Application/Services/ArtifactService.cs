using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Interface;

namespace MuseumSystem.Application.Services
{
    public class ArtifactService : IArtifactService
    {
        public Task<ArtifactResponse> CreateArtifact(ArtifactRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArtifact(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<ArtifactResponse>> GetAllArtifacts(
            int pageIndex, 
            int pageSize, 
            string? artifactCode, 
            string? name, 
            string? periodTime, 
            bool includeDeleted, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArtifactResponse> GetArtifactById(
            string id, 
            bool includeDeleted, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ArtifactResponse> UpdateArtifact(
            string id, 
            ArtifactRequest request, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
