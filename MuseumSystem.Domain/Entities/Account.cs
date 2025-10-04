using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Entities
{
    public class Account
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public EnumActive IsActive { get; set; } = EnumActive.Active;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
        // Foreign key to Role
        public string RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
