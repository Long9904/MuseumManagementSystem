
namespace MuseumSystem.Application.Dtos.AuthDtos
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string RoleId { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public string? Status { get; set; }

        public string? MuseumId { get; set; }

        public string? MuseumName { get; set; }

        public string? MuseumDescription { get; set; }

        public string? MuseumLocation { get; set; }
    }
}
