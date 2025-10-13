using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Domain.Abstractions;


namespace MuseumSystem.Application.Interfaces
{
    public interface IAreaService
    {
        Task<AreaResponse> CreateArea(AreaRequest request);
        Task<AreaResponse> UpdateArea(string id, AreaRequest request);
        Task DeleteArea(string id);
        Task<AreaResponse> GetAreaById(string id);
        Task<BasePaginatedList<AreaResponse>> GetAll(int pageIndex, int pageSize);
    }
}
