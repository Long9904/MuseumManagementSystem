using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.InteractionDtos;


namespace MuseumSystem.Application.Interfaces
{
    public interface IInteractionService
    {
        Task<ApiResponse<List<InteractionResponse>>> GetAllAsync();
        Task<ApiResponse<InteractionResponse>> GetByIdAsync(string id);
        Task<ApiResponse<InteractionResponse>> UpdateAsync(string id, InteractionUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
