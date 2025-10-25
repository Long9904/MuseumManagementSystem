using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Domain.Abstractions;


namespace MuseumSystem.Application.Interfaces
{
    public interface IAreaService
    {
        Task<AreaResponse> CreateArea(
            AreaRequest request, 
            CancellationToken cancellationToken = default);

        Task<AreaResponse> UpdateArea(
            string id, 
            AreaRequest request, 
            CancellationToken cancellationToken = default);

        Task DeleteArea(
            string id, 
            CancellationToken cancellationToken = default);

        Task<AreaResponse> GetAreaById(
            string id, 
            bool includeDeleted, 
            CancellationToken cancellationToken = default);

        Task<BasePaginatedList<AreaResponse>> GetAll(
            int pageIndex, 
            int pageSize,
            string? areaName,
            bool includeDeleted, 
            CancellationToken cancellationToken = default);

        Task ActiveArea (
            string id, 
            CancellationToken cancellationToken = default);

        Task MaintainArea (
            string id, 
            CancellationToken cancellationToken = default);
    }
}
