using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper;
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

        public async Task ActiveArtifact(string id, CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();

            Artifact artifact = await _unitOfWork.ArtifactRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Artifact with ID '{id}' not found.");

            if (artifact.MuseumId != museumId)
            {
                throw new InvalidAccessException("User does not have access to this artifact.");
            }

            artifact.Status = ArtifactStatus.InStorage;
            artifact.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ArtifactRepository.UpdateAsync(artifact);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Reactivated artifact {ArtifactCode} in museum {MuseumId}", artifact.ArtifactCode, artifact.MuseumId);
        }

        public async Task<ArtifactResponse> CreateArtifact(
            ArtifactRequest request,
            CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();


            var newArtifact = _mapper.Map<Artifact>(request);
            newArtifact.ArtifactCode = await GenerateArtifactCodeAsync(museumId);
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

        public async Task<BasePaginatedList<ArtifactResponse>> GetAllArtifacts(
            int pageIndex,
            int pageSize,
            string? name,
            string? periodTime,
            bool includeDeleted,
            CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();
            var query = _unitOfWork.ArtifactRepository.Entity
                .Include(a => a.Museum)
                .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area)
                .Where(a => a.MuseumId == museumId);

            if (!includeDeleted)
            {
                query = query.Where(a => a.Status != ArtifactStatus.Deleted);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (!string.IsNullOrEmpty(periodTime))
            {
                query = query.Where(a => a.PeriodTime.ToLower().Contains(periodTime.ToLower()));
            }

            query = query.OrderBy(a => a.ArtifactCode).ThenByDescending(a => a.UpdatedAt);

            BasePaginatedList<Artifact> paginatedList = await _unitOfWork.ArtifactRepository.GetPagging(query, pageIndex, pageSize);

            var result = _mapper.Map<BasePaginatedList<Artifact>, BasePaginatedList<ArtifactResponse>>(paginatedList);
            return result;
        }

        public async Task<ArtifactDetailsResponse> GetArtifactById(
            string id,
            bool includeDeleted,
            CancellationToken cancellationToken = default)
        {
            Artifact artifact = await _unitOfWork.ArtifactRepository.FindAsync(a => a.Id == id,
                include: source => source
                    .Include(a => a.Museum)
                    .Include(a => a.ArtifactMedias)
                    .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area))
                ?? throw new NotFoundException($"Artifact with ID '{id}' not found.");

            var museumId = await GetValidMuseumIdAsync();
            if (artifact.MuseumId != museumId)
            {
                throw new InvalidAccessException("User does not have access to this artifact.");
            }

            if (!includeDeleted && artifact.Status == ArtifactStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot access a deleted artifact.");
            }

            // Take list of medias and sort by MediaType and CreatedAt
            artifact.ArtifactMedias = artifact.ArtifactMedias
                .OrderBy(am => am.MediaType)
                .ThenByDescending(am => am.CreatedAt)
                .ToList();

            // Map to Media response

            var mediaResponses = artifact.ArtifactMedias
                .Select(m => new MediaResponse
                {
                    Id = m.Id,
                    MediaType = m.MediaType,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    MimeType = m.MimeType,
                    FileFormat = m.FileFormat,
                    Caption = m.Caption,
                    Status = m.Status,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .OrderBy(m => m.MediaType)
                .ThenByDescending(m => m.CreatedAt).ToList();

            var artifactDetailsResponse = _mapper.Map<ArtifactDetailsResponse>(artifact);
            artifactDetailsResponse.MediaItems = mediaResponses;
            return artifactDetailsResponse;
        }

        public async Task<ArtifactResponse> UpdateArtifact(
            string id,
            ArtifactRequest request,
            CancellationToken cancellationToken = default)
        {
            Artifact artifact = await ValidateArtifactAccess(id);

            _mapper.Map(request, artifact);
            artifact.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.ArtifactRepository.UpdateAsync(artifact);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Updated artifact {ArtifactCode} in museum {MuseumId}", artifact.ArtifactCode, artifact.MuseumId);
            return _mapper.Map<ArtifactResponse>(artifact);
        }

        public async Task<ArtifactResponse> AssignArtifactToDisplayPosition(
            string artifactId, string displayPositionId, CancellationToken cancellationToken = default)
        {
            Artifact artifact = await ValidateArtifactAccess(artifactId);

            var displayPosition = await _unitOfWork.DisplayPositionRepository.GetByIdAsync(displayPositionId)
                ?? throw new NotFoundException($"Display position with ID '{displayPositionId}' not found.");

            var area = await _unitOfWork.AreaRepository.GetByIdAsync(displayPosition.AreaId)
                ?? throw new NotFoundException($"Area with ID '{displayPosition.AreaId}' not found.");

            if (area.MuseumId != artifact.MuseumId)
            {
                throw new InvalidAccessException("Display position does not belong to the same museum as the artifact.");
            }

            if (displayPosition.Status == DisplayPositionStatusEnum.Deleted)
            {
                throw new ObjectDeletedException("Cannot assign artifact to a deleted display position.");
            }

            if (displayPosition.ArtifactId != null)
            {
                throw new ConflictException("Display position is already occupied by another artifact.");
            }

            if (artifact.DisplayPosition != null)
            {
                throw new ConflictException("Artifact is already assigned to a display position.");
            }

            displayPosition.ArtifactId = artifact.Id;

            await _unitOfWork.DisplayPositionRepository.UpdateAsync(displayPosition);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Assigned artifact {ArtifactCode} to display position {PositionCode} in museum {MuseumId}", artifact.ArtifactCode, displayPosition.PositionCode, artifact.MuseumId);
            return _mapper.Map<ArtifactResponse>(artifact);

        }

        public async Task<ArtifactResponse> RemoveArtifactFromDisplayPosition(
            string artifactId, CancellationToken cancellationToken = default)
        {
            Artifact artifact = await ValidateArtifactAccess(artifactId);

            DisplayPosition? displayPosition = await _unitOfWork.DisplayPositionRepository.FindAsync(dp => dp.ArtifactId == artifactId)
                      ?? throw new NotFoundException("Artifact is not assigned to any display position.");

            displayPosition.ArtifactId = null;
            displayPosition.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.DisplayPositionRepository.UpdateAsync(displayPosition);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Removed artifact {ArtifactCode} from display position {PositionCode} in museum {MuseumId}", artifact.ArtifactCode, displayPosition.PositionCode, artifact.MuseumId);
            return _mapper.Map<ArtifactResponse>(artifact);
        }


        public async Task<ArtifactResponse> GetArtifactByCode(string code, CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();

            Artifact artifact = await _unitOfWork.ArtifactRepository.FindAsync(a => a.ArtifactCode == code && a.MuseumId == museumId,
                include: source => source
                    .Include(a => a.Museum)
                    .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area))
                ?? throw new NotFoundException($"Artifact with code '{code}' not found.");

            if (artifact.Status == ArtifactStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot access a deleted artifact.");
            }

            return _mapper.Map<ArtifactResponse>(artifact);

        }

        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found or status is not Active");

            return museumId;
        }

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

        private async Task<string> GenerateArtifactCodeAsync(string museumId)
        {
            var museum = await _unitOfWork.MuseumRepository.GetByIdAsync(museumId)
                ?? throw new NotFoundException($"Museum with ID '{museumId}' not found.");

            string cleanName = RemoveDiacritics(museum.Name).Replace(" ", "");


            if (museum.Name.Length >= 3)
            {
                cleanName = cleanName.Substring(0, 3).ToUpper().Trim();
            }
            else
            {
                cleanName = cleanName.ToUpper();
            }

            int codeNumber = await _unitOfWork.ArtifactRepository.Entity
                .CountAsync(a => a.MuseumId == museumId) + 1;
            var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return $"{cleanName}-ART-{codeNumber:D4}-{datePart}";
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var chars = normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
