using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Entities
{
    public class Exhibition
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 10, ErrorMessage = "Priority must be between 0 and 10")]
        public int Priority { get; set; } = 0; // 0 là cao nhất

        public ExhibitionStatus Status { get; set; } = ExhibitionStatus.Daily; // Mặc định Daily

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Liên kết đến bảo tàng
        [Required]
        public string MuseumId { get; set; } = string.Empty;
        [JsonIgnore]
        public Museum? Museum { get; set; }

        // Liên kết đến danh sách artifact hoặc context
        [JsonIgnore]
        public ICollection<Artifact>? Artifacts { get; set; }

        [JsonIgnore]
        public ICollection<ExhibitionHistoricalContext> ExhibitionHistoricalContexts { get; set; } = new List<ExhibitionHistoricalContext>();

        public ICollection<HistoricalContext>? HistoricalContexts =>
    ExhibitionHistoricalContexts?.Select(eh => eh.HistoricalContext).ToList();

    }
}
