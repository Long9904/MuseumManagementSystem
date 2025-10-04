using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Entities
{
    public class Role
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
