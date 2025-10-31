using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IGenerateTokenService
    {
        Task<string> GenerateToken(Account acc);

        string GenerateVisitorToken(Visitor visitor);
    }
}
