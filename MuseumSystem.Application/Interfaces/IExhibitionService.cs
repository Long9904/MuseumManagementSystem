using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Domain.Entities;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IExhibitionService
    {
        Task<ApiResponse<ExhibitionResponse>> CreateAsync(ExhibitionRequest request);
        Task<ApiResponse<ExhibitionResponse>> UpdateAsync(string id, ExhibitionUpdateRequest request);
        Task<ApiResponse<List<ExhibitionResponse>>> GetAllAsync();
        Task<ApiResponse<ExhibitionResponse>> GetByIdAsync(string id);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
