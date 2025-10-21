using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IVisitorService
    {
        Task<ApiResponse<List<VisitorResponse>>> GetAllAsync();
        Task<ApiResponse<VisitorResponse>> GetByIdAsync(string id);
        Task<ApiResponse<VisitorResponse>> CreateAsync(VisitorRequest request);
        Task<ApiResponse<VisitorResponse>> UpdateAsync(string id, VisitorUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
