using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IInteractionService
    {
        Task<ApiResponse<List<InteractionResponse>>> GetAllAsync();
        Task<ApiResponse<InteractionResponse>> GetByIdAsync(string id);
        Task<ApiResponse<InteractionResponse>> CreateAsync(InteractionRequest request);
        Task<ApiResponse<InteractionResponse>> UpdateAsync(string id, InteractionUpdateRequest request);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
