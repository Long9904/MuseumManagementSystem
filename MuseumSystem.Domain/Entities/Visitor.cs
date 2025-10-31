
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class Visitor
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public VisitorStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    }
}
