using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Application.Interfaces
{
    public interface IVisitorService
    {
        Task<VisitorResponse> RegisterVisitorAsync(VisitorRequest visitorRequest);

        Task<VisitorLoginResponse> LoginVisitorAsync(VisitorRequest visitorRequest);

        Task<VisitorResponse> MyProfileAsync();

        // Post interactions like comments, ratings, etc. can be added here in the future
        Task<VisitorInteractionResponse> PostInteractionAsync(InteractionRequest request);

        // My interactions
        Task<BasePaginatedList<VisitorInteractionResponse>> MyInteractionsAsync(int pageIndex, int pageSize);
    }
}
