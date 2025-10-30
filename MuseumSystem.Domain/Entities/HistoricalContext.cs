using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseumSystem.Domain.Entities
{
    public class HistoricalContext
    {
        [Key]
        public string HistoricalContextId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string? Period { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        // Navigation
        public ICollection<ArtifactHistoricalContext>? ArtifactHistoricalContexts { get; set; }
        public ICollection<ExhibitionHistoricalContext>? ExhibitionHistoricalContexts { get; set; }
    }
}
