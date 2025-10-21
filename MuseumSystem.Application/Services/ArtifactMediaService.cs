using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Services
{
    public class ArtifactMediaService : IArtifactMediaService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly CloudStorageService _storageService;
        private readonly ILogger<ArtifactMediaService> _logger;



        private async Task<Artifact> ValidateArtifactAccess(string artifactId)
        {
            var museumId = await GetValidMuseumIdAsync();

            Artifact artifact = await _unitOfWork.ArtifactRepository.GetByIdAsync(artifactId)
                ?? throw new NotFoundException($"Artifact with ID '{artifactId}' not found.");

            if (artifact.MuseumId != museumId)
            {
                throw new InvalidAccessException("User does not have access to this artifact.");
            }

            if (artifact.Status == ArtifactStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot access a deleted artifact.");
            }

            return artifact;
        }


        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found or status is not Active");

            return museumId;
        }
    }
}
