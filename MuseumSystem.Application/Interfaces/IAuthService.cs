using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AuthDtos;


namespace MuseumSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(AuthRequest request);
        Task<AuthResponse> LoginGoogleAsync(LoginGGRequest loginGGRequest);
        Task Logout();
        Task<UserProfileResponse> GetCurrentUserProfileAsync();

        Task<AccountRespone> RegisterAccountWithMuseumAsync(RegisterRequest request);
    }
}
