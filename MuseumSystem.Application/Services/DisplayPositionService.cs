using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.DisplayPositionDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Services
{
    public class DisplayPositionService : IDisplayPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapping;
        private readonly ILogger<DisplayPositionService> _logger;

        public DisplayPositionService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapping,
            ILogger<DisplayPositionService> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapping = mapping;
            _logger = logger;
        }


        public async Task<DisplayPositionResponse> CreateDisplayPosition(DisplayPositionRequest request, CancellationToken cancellationToken = default)
        {
            await ValidateMuseumAccess(request.AreaId);
            await ValidateUniquePositionCodeAsync(request.AreaId, request.PositionCode);

            var newDisplayPosition = _mapping.Map<DisplayPosition>(request);

            newDisplayPosition.Status = DisplayPositionStatusEnum.Active;
            newDisplayPosition.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.DisplayPositionRepository.InsertAsync(newDisplayPosition);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Created new display position {PositionCode} in area {AreaId}", newDisplayPosition.PositionCode, newDisplayPosition.AreaId);

            return _mapping.Map<DisplayPositionResponse>(newDisplayPosition);
        }


        public async Task DeleteDisplayPosition(string id, CancellationToken cancellationToken = default)
        {
            var displayPosition = await _unitOfWork.DisplayPositionRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Display position with ID '{id}' not found.");

            await ValidateMuseumAccess(displayPosition.AreaId);

            displayPosition.Status = DisplayPositionStatusEnum.Deleted;
            displayPosition.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.DisplayPositionRepository.UpdateAsync(displayPosition);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Deleted display position {PositionCode} in area {AreaId}", displayPosition.PositionCode, displayPosition.AreaId);
        }


        public async Task<BasePaginatedList<DisplayPositionResponse>> GetAllDisplayPositions(
            int pageIndex,
            int pageSize,
            string? artifactName,
            string? displayPositionName,
            string? areaName,
            bool includeDeleted,
            CancellationToken cancellationToken = default)
        {
            var museumId = await GetValidMuseumIdAsync();

            var query = _unitOfWork.DisplayPositionRepository.Entity
                .Include(dp => dp.Area)
                .Include(dp => dp.Artifact)
                .Where(dp => dp.Area.MuseumId == museumId);

            if (!includeDeleted)
            {
                query = query.Where(dp => dp.Status != DisplayPositionStatusEnum.Deleted);
            }

            if (!string.IsNullOrWhiteSpace(artifactName))
            {
                query = query.Where(dp => dp.Artifact != null && dp.Artifact.Name.ToLower().Contains(artifactName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(displayPositionName))
            {
                query = query.Where(dp => dp.DisplayPositionName.ToLower().Contains(displayPositionName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                query = query.Where(dp => dp.Area.Name.ToLower().Contains(areaName.ToLower()));
            }

            query = query
                .OrderBy(dp => dp.Area.Name)
                .ThenBy(dp => dp.PositionCode)
                .ThenByDescending(dp => dp.UpdatedAt);

            BasePaginatedList<DisplayPosition> paginatedList = await _unitOfWork.DisplayPositionRepository.GetPagging(query, pageIndex, pageSize);

            var result = _mapping.Map<BasePaginatedList<DisplayPosition>, BasePaginatedList<DisplayPositionResponse>>(paginatedList);

            return result;
        }


        public async Task<DisplayPositionResponse> GetDisplayPositionById(string id,bool includeDeleted, CancellationToken cancellationToken = default)
        {
            DisplayPosition displayPosition = await _unitOfWork.DisplayPositionRepository.FindAsync(dp =>
            dp.Id == id
            && (includeDeleted || dp.Status != DisplayPositionStatusEnum.Deleted),
            include: source => source
            .Include(dp => dp.Area)
            .Include(dp => dp.Artifact))
                ?? throw new NotFoundException($"Display position with ID '{id}' not found.");

            await ValidateMuseumAccess(displayPosition.AreaId);

            return _mapping.Map<DisplayPositionResponse>(displayPosition);
        }


        public async Task<DisplayPositionResponse> UpdateDisplayPosition(string id, DisplayPositionRequest request, CancellationToken cancellationToken = default)
        {
            DisplayPosition existingPosition = await _unitOfWork.DisplayPositionRepository.FindAsync(dp =>
            dp.Id == id,
            include: source => source
            .Include(dp => dp.Area)
            .Include(dp => dp.Artifact))
                ?? throw new NotFoundException($"Display position with ID '{id}' not found.");

            if (existingPosition.Status == DisplayPositionStatusEnum.Deleted)
            {
                throw new ObjectDeletedException("Cannot update a deleted display position.");
            }

            await ValidateMuseumAccess(existingPosition.AreaId);

            bool isAreaChanged = !request.AreaId.Equals(existingPosition.AreaId, StringComparison.OrdinalIgnoreCase);
            bool isCodeChanged = !request.PositionCode.Equals(existingPosition.PositionCode, StringComparison.OrdinalIgnoreCase);

            if (isAreaChanged || isCodeChanged)
            {
                await ValidateUniquePositionCodeAsync(request.AreaId, request.PositionCode);
            }

            existingPosition.DisplayPositionName = request.DisplayPositionName;
            existingPosition.PositionCode = request.PositionCode;
            existingPosition.Description = request.Description;
            existingPosition.UpdatedAt = DateTime.UtcNow;
            existingPosition.AreaId = request.AreaId;

            await _unitOfWork.DisplayPositionRepository.UpdateAsync(existingPosition);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Updated display position {PositionCode} in area {AreaId}", existingPosition.PositionCode, existingPosition.AreaId);

            return _mapping.Map<DisplayPositionResponse>(existingPosition);
        }


        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found.");

            return museumId;
        }


        private async Task ValidateMuseumAccess(string areaId)
        {
            // 1. Find the area by id
            var area = await _unitOfWork.AreaRepository.GetByIdAsync(areaId)
                ?? throw new NotFoundException($"Area with ID '{areaId}' not found.");

            // 2. Check if the area is deleted
            if (area.Status == AreaStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot access a deleted area.");
            }

            // 3. Get the museumId associated with the current user
            var museumId = await GetValidMuseumIdAsync();

            if (area.MuseumId != museumId)
            {
                _logger.LogWarning("User {UserId} attempted to access area {AreaId} of museum {MuseumId} without permission.", _currentUserService.UserId, areaId, museumId);
                throw new InvalidAccessException("You do not have permission to access this area.");
            }
        }


        private async Task ValidateUniquePositionCodeAsync(string areaId, string positionCode)
        {
            // Only unique with the same museum & with the same area
            var existingPosition = await _unitOfWork.DisplayPositionRepository.FindAsync(dp =>
            dp.AreaId == areaId
            && dp.PositionCode == positionCode
            && dp.Status != DisplayPositionStatusEnum.Deleted,
            include: source => source.Include(dp => dp.Area)
            );

            if (existingPosition != null)
            {
                throw new ConflictException($"A display position with code '{positionCode}' already exists in this area {existingPosition.Area.Name}.");
            }
        }
    }
}
