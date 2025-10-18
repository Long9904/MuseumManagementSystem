using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using MuseumSystem.Domain.Interface;

namespace MuseumSystem.Application.Services
{
    public class ArtifactService : IArtifactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ArtifactService> _logger;
        private readonly IMapper _mapper;

        public ArtifactService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<ArtifactService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ArtifactResponse> CreateArtifact(
            ArtifactRequest request,
            CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();
            await ValidateArtifactCodeAsync(request.ArtifactCode, museumId, null);

            var newArtifact = _mapper.Map<Artifact>(request);
            newArtifact.MuseumId = museumId;
            newArtifact.Status = ArtifactStatus.InStorage;
            newArtifact.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ArtifactRepository.InsertAsync(newArtifact);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Created new artifact {ArtifactCode} in museum {MuseumId}", newArtifact.ArtifactCode, museumId);
            return _mapper.Map<ArtifactResponse>(newArtifact);
        }

        public async Task DeleteArtifact(string id, CancellationToken cancellationToken = default)
        {
            Artifact artifact = await _unitOfWork.ArtifactRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Artifact with ID '{id}' not found.");

            var museumId = await GetValidMuseumIdAsync();
            if (artifact.MuseumId != museumId)
            {
                throw new InvalidAccessException("User does not have access to this artifact.");
            }

            if (artifact.Status == ArtifactStatus.Deleted)
            {
                throw new ObjectDeletedException("Artifact is already deleted.");
            }

            if (artifact.DisplayPosition != null)
            {
                throw new ConflictException("Cannot delete an artifact that is currently on display.");
            }

            artifact.Status = ArtifactStatus.Deleted;
            artifact.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ArtifactRepository.UpdateAsync(artifact);
            await _unitOfWork.SaveChangeAsync();
                        _logger.LogInformation("Deleted artifact {ArtifactCode} in museum {MuseumId}", artifact.ArtifactCode, museumId);
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

        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found or status is not Active");

            return museumId;
        }

        private async Task ValidateArtifactCodeAsync(string artifactCode, string museumId, string? existingArtifactId = null)
        {
            var artifact = await _unitOfWork.ArtifactRepository
                .FindAsync(a => a.ArtifactCode == artifactCode
                && (existingArtifactId == null || a.Id != existingArtifactId)
                && a.MuseumId == museumId,
                include: source => source.Include(a => a.Museum));

            if (artifact != null)
            {
                throw new ConflictException($"Artifact with code '{artifactCode}' already exists.");
            }
        }
    }
}
