using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseumSystem.Domain.Entities
{
    public class Visitor
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Phone]
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public EnumStatus Status { get; set; } = EnumStatus.Active; // ✅ Enum, không string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    }
}
