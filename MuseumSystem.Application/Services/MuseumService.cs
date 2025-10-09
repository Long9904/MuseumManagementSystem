using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class MuseumService : IMuseumService
    {
        private readonly IUnitOfWork unit;
        private readonly ILogger<MuseumService> logger;

        public MuseumService(IUnitOfWork unit, ILogger<MuseumService> logger)
        {
            this.unit = unit;
            this.logger = logger;
        }

        public async Task<Museum> CreateMuseum(MuseumRequest museumDto)
        {
            var museum = new Museum
            {
                Name = museumDto.Name,
                Location = museumDto.Location,
                Description = museumDto.Description,
                Status = EnumStatus.Active,
                CreateAt = DateTime.UtcNow
            };
            await unit.GetRepository<Museum>().InsertAsync(museum);
            await unit.SaveChangeAsync();
            logger.LogInformation("Museum {MuseumName} created successfully.", museumDto.Name);
            return museum;
        }

        public async Task DeleteMuseum(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            var museum = await unit.GetRepository<Museum>().FindAsync(x => x.Id == id);
            if (museum == null)
            {
                throw new KeyNotFoundException($"Museum with ID {id} not found.");
            }
            museum.Status = EnumStatus.Inactive;
            await unit.GetRepository<Museum>().UpdateAsync(museum);
            await unit.SaveChangeAsync();
        }

        public async Task<BasePaginatedList<Museum>> GetAll(int pageIndex, int pageSize)
        {
            var query = unit.GetRepository<Museum>().Entity;
            return await unit.GetRepository<Museum>().GetPagging(query, pageIndex, pageSize);
        }

        public Task<Museum?> GetMuseumById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            return unit.GetRepository<Museum>().FindAsync(x => x.Id == id && x.Status == EnumStatus.Active);
        }

        public async Task<Museum> UpdateMuseum(string id, MuseumRequest museumDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            if (museumDto == null)
            {
                throw new ArgumentNullException(nameof(museumDto), "Museum request cannot be null.");
            }
            bool isUpdated = false;
            var museum = await unit.GetRepository<Museum>().FindAsync(x => x.Id == id);
            if (museum == null)
            {
                throw new KeyNotFoundException($"Museum with ID {id} not found.");
            }
            if (museumDto.Name != null && museumDto.Name != museum.Name)
            {
                museum.Name = museumDto.Name;
                isUpdated = true;
            }
            if (museumDto.Location != null && museumDto.Location != museum.Location)
            {
                museum.Location = museumDto.Location;
                isUpdated = true;
            }
            if (museumDto.Description != null && museumDto.Description != museum.Description)
            {
                museum.Description = museumDto.Description;
                isUpdated = true;
            }
            if (isUpdated)
            {
                museum.UpdateAt = DateTime.UtcNow;
                await unit.GetRepository<Museum>().UpdateAsync(museum);
                await unit.SaveChangeAsync();
                logger.LogInformation("Museum with ID {MuseumId} updated successfully.", id);
            }
            return museum;
        }
    }
}
