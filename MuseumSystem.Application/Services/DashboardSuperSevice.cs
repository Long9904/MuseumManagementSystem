using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class DashboardService : IDashboardSuperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<object> GetAccountStatsAsync()
        {
            //Get total active users
            var userAvailableCount = _unitOfWork.GetRepository<Account>().FilterByAsync(x => x.Status == EnumStatus.Active).Result.Count;
            //Get account by role
            var roles = await _unitOfWork.GetRepository<Role>().FilterByAsync(x => x.Status == EnumStatus.Active);
            var accountByRole = new Dictionary<string, object>();
            foreach (var role in roles)
            {
                var accountsInRole = await _unitOfWork.GetRepository<Account>().FilterByAsync(x => x.RoleId == role.Id && x.Status == EnumStatus.Active);
                var countByRole = accountsInRole.Count;
                accountByRole.Add(role.Name, new
                {
                    Count = countByRole,
                });
            }
            //Get new accounts in last 7 days
            var today = DateTime.UtcNow;
            var getAccountsLast7Days = _unitOfWork.GetRepository<Account>().Entity.Include(x => x.Role).Include(x => x.Museum);
            var rs = await _unitOfWork.GetRepository<Account>().GetPagging(getAccountsLast7Days.Where(x => x.CreateAt >= today.AddDays(-7) && x.Status == EnumStatus.Active && x.Role.Name != "SuperAdmin"), 1, 7);
            var accountsPage = _mapper.Map<BasePaginatedList<AccountRespone>>(rs);
            var accountStats = new
            {
                TotalActiveUsers = userAvailableCount,
                AccountByRole = accountByRole,
                NewAccountsLast7Days = accountsPage
            };
            return accountStats;



        }

        public async Task<object> GetArtifactStatsAsync()
        {
            var totalArtifactCount = await _unitOfWork.GetRepository<Artifact>().Entity.CountAsync();
            //Get artifacts by status
            var artifactsByStatus = new Dictionary<string, object>();
            foreach( var status in Enum.GetValues(typeof(ArtifactStatus)).Cast<ArtifactStatus>())
            {
                var artifactsInStatus = await _unitOfWork.GetRepository<Artifact>().FilterByAsync(x => x.Status == status);
                artifactsByStatus.Add(status.ToString(), new
                {
                    Count = artifactsInStatus.Count,
                });
            }
            var artifactStats = new
            {
                TotalArtifacts = totalArtifactCount,
                ArtifactsByStatus = artifactsByStatus
            };
            return artifactStats;
        }

        public async Task<object> GetMuseumStatsAsync()
        {
            var totalMuseumCount = await _unitOfWork.GetRepository<Museum>().Entity.CountAsync();
            //Get museums by status
            var museumsByStatus = new Dictionary<string, object>();
            foreach (var status in Enum.GetValues(typeof(EnumStatus)).Cast<EnumStatus>())
            {
                var museumsInStatus = await _unitOfWork.GetRepository<Museum>().FilterByAsync(x => x.Status == status);
                museumsByStatus.Add(status.ToString(), new
                {
                    Count = museumsInStatus.Count,
                });
            }
            // Get museum with most artifacts
            var museumWithMostArtifacts = _unitOfWork.GetRepository<Museum>().Entity
                .Select(m => new
                {
                    Museum = m,
                    ArtifactCount = _unitOfWork.GetRepository<Artifact>().Entity.Count(a => a.MuseumId == m.Id && a.Status != ArtifactStatus.Deleted)
                })
                .OrderByDescending(ma => ma.ArtifactCount)
                .FirstOrDefault();
            var museumStats = new
            {
                TotalMuseums = totalMuseumCount,
                MuseumsByStatus = museumsByStatus,
                MuseumWithMostArtifacts = museumWithMostArtifacts != null ? new
                {
                    MuseumId = museumWithMostArtifacts.Museum.Id,
                    MuseumName = museumWithMostArtifacts.Museum.Name,
                    ArtifactCount = museumWithMostArtifacts.ArtifactCount
                } : null
            };
            return museumStats;
        }

       
    }
}
