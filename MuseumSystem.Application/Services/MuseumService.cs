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

        public async Task<Museum> RegisterMuseum(MuseumRequest museumDto)
        {
            var existingMuseum = await unit.GetRepository<Museum>().FindAsync(x => x.Name == museumDto.Name);
            if (existingMuseum != null)
            {
                throw new InvalidOperationException($"A museum with name {museumDto.Name} already exists.");
            }
            var museum = new Museum
            {
                Name = museumDto.Name,
                Location = museumDto.Location,
                Description = museumDto.Description,
                Status = EnumStatus.Pending,
                CreateAt = DateTime.UtcNow
            };
            await unit.GetRepository<Museum>().InsertAsync(museum);
            await unit.SaveChangeAsync();
            logger.LogInformation("Museum {MuseumName} registered successfully.", museumDto.Name);
            return museum;
        }

        public async Task DeleteMuseum(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            var museum = await GetMuseumById(id);
            museum.Status = EnumStatus.Inactive;
            await unit.GetRepository<Museum>().UpdateAsync(museum);
            await unit.SaveChangeAsync();
        }
        public async Task ActiveMuseum(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            var museum = await GetMuseumById(id);
            museum.Status = EnumStatus.Active;
            await unit.GetRepository<Museum>().UpdateAsync(museum);
            await unit.SaveChangeAsync();
        }

        public async Task<BasePaginatedList<Museum>> GetAll(int pageIndex, int pageSize, MuseumFilterDtos? dtos = null)
        {
            var query = unit.GetRepository<Museum>().Entity;

            if (dtos != null)
            {
                if (!string.IsNullOrWhiteSpace(dtos.Name))
                {
                    query = query.Where(x => x.Name.Contains(dtos.Name));
                }
                if (!string.IsNullOrWhiteSpace(dtos.Location))
                {
                    query = query.Where(x => x.Location.Contains(dtos.Location));
                }
                if (!string.IsNullOrWhiteSpace(dtos.Description))
                {
                    query = query.Where(x => x.Description.Contains(dtos.Description));
                }
                if (dtos.Status != 0)
                {
                    query = query.Where(x => x.Status == dtos.Status);
                }
                if (dtos.Orderby != 0)
                {
                    if (dtos.Orderby == EnumOrderBy.Asc)
                    {
                        query = query.OrderBy(x => x.CreateAt);
                    }
                    else if (dtos.Orderby == EnumOrderBy.Desc)
                    {
                        query = query.OrderByDescending(x => x.CreateAt);
                    }
                }
            }

            return await unit.GetRepository<Museum>().GetPagging(query, pageIndex, pageSize);
        }

        public async Task<Museum> GetMuseumById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum ID cannot be null or empty.", nameof(id));
            }
            var museum = await unit.GetRepository<Museum>().FindAsync(x => x.Id == id && x.Status == EnumStatus.Active);
            if (museum == null)
            {
                throw new KeyNotFoundException($"Museum with ID {id} not found.");
            }
            return museum;
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
            var museum = await GetMuseumById(id);
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
