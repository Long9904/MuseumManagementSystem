using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IMuseumService
    {
        Task<Museum> CreateMuseum(MuseumRequest museumDto);
        Task<Museum> UpdateMuseum(string id, MuseumRequest museumDto);
        Task<BasePaginatedList<Museum>> GetAll(int pageIndex, int pageSize, MuseumFilterDtos? dtos);
        Task DeleteMuseum(string id);
        Task<Museum> GetMuseumById(string id);

    }
}
