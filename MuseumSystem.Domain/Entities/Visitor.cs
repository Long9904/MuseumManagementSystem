using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseumSystem.Domain.Entities
{
    public class Visitor
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // ✅ Đổi VisitorId -> Id

        [Phone]
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    }
}
