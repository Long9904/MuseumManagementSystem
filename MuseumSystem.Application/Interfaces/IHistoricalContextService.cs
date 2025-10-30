using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.HistoricalContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IHistoricalContextService
    {
        Task<ApiResponse<List<HistoricalContextResponse>>> GetAllAsync();
        Task<ApiResponse<HistoricalContextResponse>> GetByIdAsync(string id);
        Task<ApiResponse<HistoricalContextResponse>> CreateAsync(HistoricalContextRequest request);
        Task<ApiResponse<HistoricalContextResponse>> UpdateAsync(string id, HistoricalContextUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
