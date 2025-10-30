using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumSystem.Domain.Entities
{
    public class Interaction
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string VisitorId { get; set; } // FK đến Visitor.Id

        [Required]
        public string ArtifactId { get; set; }

        public string? Comment { get; set; }

        public double? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(VisitorId))]
        public virtual Visitor Visitor { get; set; }

        [ForeignKey(nameof(ArtifactId))]
        public virtual Artifact Artifact { get; set; }
    }
}
