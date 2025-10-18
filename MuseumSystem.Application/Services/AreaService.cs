using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly ILogger<AreaService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapping;

        public AreaService(
            ILogger<AreaService> logger,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapping)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapping = mapping;
        }

        public async Task<AreaResponse> CreateArea(AreaRequest request, CancellationToken cancellationToken)
        {
            var museumId = await GetValidMuseumIdAsync();

            await ValidateUniqueAreaNameAsync(museumId, request.Name);

            var newArea = _mapping.Map<Area>(request);
            newArea.MuseumId = museumId;
            newArea.Status = AreaStatus.Active;
            newArea.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.AreaRepository.InsertAsync(newArea);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Created new area {AreaName} for museum {MuseumId}", newArea.Name, museumId);

            return _mapping.Map<AreaResponse>(newArea);
        }

        public async Task DeleteArea(string id, CancellationToken cancellationToken)
        {
            Area area = await _unitOfWork.AreaRepository.FindAsync(a => id.Equals(a.Id)) ?? throw new KeyNotFoundException($"Area not found.");
            area.Status = AreaStatus.Deleted;
            area.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.AreaRepository.UpdateAsync(area);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Deleted area {AreaName} for museum {MuseumId}", area.Name, area.MuseumId);
        }

        public async Task<BasePaginatedList<AreaResponse>> GetAll(
            int pageIndex, 
            int pageSize,
            string? areaName,
            bool includeDeleted, 
            CancellationToken cancellationToken)
        {
            var museumId = await GetValidMuseumIdAsync();

            var query = _unitOfWork.AreaRepository.Entity.Include(a => a.DisplayPositions).Where(a => a.MuseumId == museumId);

            if (!includeDeleted)
            {
                query = query.Where(a => a.Status != AreaStatus.Deleted);
            }

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                query = query.Where(a => a.Name.ToLower().Contains(areaName.ToLower()));
            }

            query = query
                .OrderBy(a => a.Status)
                .ThenByDescending(a => a.UpdatedAt)
                .ThenBy(a => a.Name);

            BasePaginatedList<Area> paginatedAreas = await _unitOfWork.AreaRepository.GetPagging(query, pageIndex, pageSize);

            // Map entities to DTOs
            var result = _mapping.Map<BasePaginatedList<Area>, BasePaginatedList<AreaResponse>>(paginatedAreas);
            return result;

        }

        public async Task<AreaResponse> GetAreaById(string id, bool includeDeleted, CancellationToken cancellationToken)
        {

            var museumId = await GetValidMuseumIdAsync();

            Area area = await _unitOfWork.AreaRepository.FindAsync
                 (a => id == a.Id
                 && museumId == a.MuseumId
                 && (includeDeleted || a.Status != AreaStatus.Deleted),
                 include: source => source.Include(a => a.DisplayPositions))
                 ?? throw new NotFoundException($"Area not found or status is deleted.");
            return _mapping.Map<AreaResponse>(area);
        }

        public async Task<AreaResponse> UpdateArea(string id, AreaRequest request, CancellationToken cancellationToken)
        {
            Area area = await _unitOfWork.AreaRepository.GetByIdAsync(id)
               ?? throw new NotFoundException($"Area with ID '{id}' not found.");

            if (area.Status == AreaStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot update a deleted area.");
            }

            var museumId = await GetValidMuseumIdAsync();
            if (area.MuseumId != museumId)
            {
                throw new InvalidAccessException("You do not have permission to update this area.");
            }

            if (!area.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                await ValidateUniqueAreaNameAsync(museumId, request.Name);
            }

            area.Name = request.Name;
            area.Description = request.Description;
            area.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.AreaRepository.UpdateAsync(area);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Updated area {AreaName} for museum {MuseumId}", area.Name, area.MuseumId);

            return _mapping.Map<AreaResponse>(area);
        }

        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found or status is not Active");

            return museumId;
        }

        private async Task ValidateUniqueAreaNameAsync(string museumId, string museumName, CancellationToken cancellationToken = default)
        {
            var existingArea = await _unitOfWork.AreaRepository.FindAsync(a => a.MuseumId == museumId && a.Name == museumName && a.Status != AreaStatus.Deleted);
            if (existingArea != null)
            {
                throw new ConflictException($"An area with the name '{museumName}' already exists in this museum.");
            }
        }
    }
}
