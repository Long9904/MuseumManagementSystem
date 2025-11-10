

namespace MuseumSystem.Application.Dtos.AccountDtos
{
    public class AccountRequestV2
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }

        public string? RoleId { get; set; }

        public string? MuseumId { get; set; }
    }
}
