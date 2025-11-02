using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IExhibitionService
    {
        Task<ApiResponse<BasePaginatedList<ExhibitionResponse>>> GetAllAsync(
    int pageNumber = 1,
    int pageSize = 10,
    ExhibitionStatus? statusFilter = null);
        Task<ApiResponse<ExhibitionResponse>> GetByIdAsync(string id);
        Task<ApiResponse<ExhibitionResponse>> CreateAsync(ExhibitionRequest request);
        Task<ApiResponse<ExhibitionResponse>> UpdateAsync(string id, ExhibitionUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);
        Task<ApiResponse<bool>> AssignHistoricalContextsAsync(string exhibitionId, ExhibitionHistoricalAssignRequest request);
        Task<ApiResponse<bool>> RemoveHistoricalContextsAsync(string exhibitionId, ExhibitionHistoricalAssignRequest request);
    }
}
