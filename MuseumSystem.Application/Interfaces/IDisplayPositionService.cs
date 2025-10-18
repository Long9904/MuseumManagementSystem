using MuseumSystem.Application.Dtos.DisplayPositionDtos;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Application.Interfaces
{
    public interface IDisplayPositionService
    {
        Task<DisplayPositionResponse> CreateDisplayPosition(
            DisplayPositionRequest request, 
            CancellationToken cancellationToken = default);

        Task<DisplayPositionResponse> UpdateDisplayPosition(
            string id, 
            DisplayPositionRequest request, 
            CancellationToken cancellationToken = default);

        Task<DisplayPositionResponse> GetDisplayPositionById(
            string id, 
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task<BasePaginatedList<DisplayPositionResponse>> GetAllDisplayPositions(
            int pageIndex,
            int pageSize,
            string? artifactName,
            string? displayPositionName,
            string? areaName,
            bool includeDeleted,
            CancellationToken cancellationToken = default);

        Task DeleteDisplayPosition(
            string id, 
            CancellationToken cancellationToken = default);

    }
}
