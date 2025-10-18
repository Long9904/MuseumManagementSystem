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
    public class Museum
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public EnumStatus Status { get; set; } = EnumStatus.Active;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }

        // Navigation property for related accounts
        [JsonIgnore]
        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        [JsonIgnore]
        public ICollection<Area> Areas { get; set; } = new List<Area>();

        [JsonIgnore]
        public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
    }
}
