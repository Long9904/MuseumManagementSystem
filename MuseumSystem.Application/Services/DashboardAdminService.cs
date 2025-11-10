using Microsoft.EntityFrameworkCore;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;


namespace MuseumSystem.Application.Services
{
    public class DashboardAdminService : IDashboardAdminService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        public DashboardAdminService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<object> GetArtifactStatsAsync()
        {
            
            var musuemId = _currentUser.MuseumId;
            var totalArtifactCount = await _unitOfWork.GetRepository<Artifact>().Entity.Where(x => x.MuseumId == musuemId).CountAsync();

            //Get list artifacts by status
            
            var artifactsByStatus = new Dictionary<string, object>();
            foreach (var status in Enum.GetValues(typeof(ArtifactStatus)).Cast<ArtifactStatus>())
            {
                var artifactsInStatus = await _unitOfWork.GetRepository<Artifact>().FilterByAsync(x => x.Status == status && x.MuseumId == musuemId );
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

        public async Task<object> GetStaffStatsAsync()
        {
            var musuemId = _currentUser.MuseumId;
            var staffInMuseum = await _unitOfWork.GetRepository<Account>().FilterByAsync(x => x.MuseumId == musuemId && x.Status == EnumStatus.Active && x.Role.Name == "Staff");
            if (staffInMuseum == null)
            {
                throw new Exception("Staff role not found");
            }
            var totalStaffCount = staffInMuseum.Count;
            var staffStats = new
            {
                TotalStaff = totalStaffCount,
            };
            return staffStats;
        }
    }
}
