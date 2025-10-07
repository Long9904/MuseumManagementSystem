using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.AccountDtos
{
    public class AccountRequest
    {     
        public string Email { get; set; }    
        public string Password { get; set; } 
        public string? FullName { get; set; }
    }
}
