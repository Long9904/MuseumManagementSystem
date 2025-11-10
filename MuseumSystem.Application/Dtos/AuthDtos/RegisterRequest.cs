namespace MuseumSystem.Application.Dtos.AuthDtos
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }

        public required string MuseumName { get; set; }

        public required string MuseumLocation { get; set; }

        public required string MuseumDescription { get; set; }
    }
}
