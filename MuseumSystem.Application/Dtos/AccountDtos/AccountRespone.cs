using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.AccountDtos
{
    public class AccountRespone
    {
        public string? Id { get; set; } 
        public string? Email { get; set; } 
        public string? FullName { get; set; }
        public EnumStatus Status { get; set; } 
        public DateTime CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; }
        public string? RoleId { get; set; }
        public string? MuseumId { get; set; }
    }
}
