using AutoMapper;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AreaDtos;
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
        private readonly ICurrentUserLogin _getCurrentUserLogin;
        private readonly IMapper _mapping;

        public AreaService(
            ILogger<AreaService> logger, 
            IUnitOfWork unitOfWork, 
            ICurrentUserLogin getCurrentUserLogin, 
            IMapper mapping)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _getCurrentUserLogin = getCurrentUserLogin;
            _mapping = mapping;
        }

        public async Task<AreaResponse> CreateArea(AreaRequest request)
        {
            var museumId = await ValidateAndGetMuseumIdAsync(request.Name);

            var newArea = _mapping.Map<Area>(request);
            newArea.MuseumId = museumId;
            newArea.Status = AreaStatus.Active;
            newArea.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.AreaRepository.InsertAsync(newArea);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Created new area {AreaName} for museum {MuseumId}", newArea.Name, museumId);

            return _mapping.Map<AreaResponse>(newArea);
        }

        public Task DeleteArea(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<AreaResponse>> GetAll(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<AreaResponse> GetAreaById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<AreaResponse> UpdateArea(string id, AreaRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task<string> ValidateAndGetMuseumIdAsync(string name)
        {
            var userId = _getCurrentUserLogin.UserId
                ?? throw new UnauthorizedAccessException("User is not logged in.");

            var account = await _unitOfWork.AccountRepository.FindAsync(a => a.Id == userId)
                ?? throw new KeyNotFoundException($"Account with ID {userId} not found.");

            var museumId = account.MuseumId
                ?? throw new InvalidOperationException("The logged-in user is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new KeyNotFoundException($"Museum not found.");

            // Validate area name uniqueness within the museum
            var existingArea = await _unitOfWork.AreaRepository.FindAsync(a => a.Name == name && a.MuseumId == museumId);
            if (existingArea != null)
            {
                throw new InvalidOperationException($"An area with the name '{name}' already exists in this museum {museum.Name}.");
            }

            return museumId;
        }
    }
}
