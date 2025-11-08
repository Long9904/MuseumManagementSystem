using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.HistoricalContextsDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IHistoricalContextService
    {
        Task<ApiResponse<BasePaginatedList<HistoricalContextResponse>>> GetAllAsync(
            int pageNumber = 1, int pageSize = 10, HistoricalStatus? statusFilter = null);

        Task<ApiResponse<HistoricalContextResponse>> GetByIdAsync(string id);
        Task<ApiResponse<HistoricalContextResponse>> CreateAsync(HistoricalContextRequest request);
        Task<ApiResponse<HistoricalContextResponse>> UpdateAsync(string id, HistoricalContextUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);

        Task<ApiResponse<bool>> AssignArtifactsAsync(string historicalContextId, HistoricalArtifactAssignRequest request);
        Task<ApiResponse<bool>> RemoveArtifactsAsync(string historicalContextId, HistoricalArtifactAssignRequest request);
    }
}
