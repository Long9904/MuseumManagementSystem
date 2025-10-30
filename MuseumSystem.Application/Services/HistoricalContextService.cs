using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.HistoricalContexts;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class HistoricalContextService : IHistoricalContextService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoricalContextService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }

        public async Task<ApiResponse<List<HistoricalContextResponse>>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();
            var contexts = await repo
                .GetQueryable()
                .Include(x => x.ArtifactHistoricalContexts)
                    .ThenInclude(x => x.Artifact)
                .Include(x => x.ExhibitionHistoricalContexts)
                    .ThenInclude(x => x.Exhibition)
                .ToListAsync();

            var result = contexts.Select(e => new HistoricalContextResponse(e)).ToList();

            return ApiResponse<List<HistoricalContextResponse>>.OkResponse(result, "Get all historical contexts successfully", "200");
        }

        public async Task<ApiResponse<HistoricalContextResponse>> GetByIdAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();
            var context = await repo
                .GetQueryable()
                .Include(x => x.ArtifactHistoricalContexts)
                    .ThenInclude(x => x.Artifact)
                .Include(x => x.ExhibitionHistoricalContexts)
                    .ThenInclude(x => x.Exhibition)
                .FirstOrDefaultAsync(x => x.HistoricalContextId == id);

            if (context == null)
                return ApiResponse<HistoricalContextResponse>.NotFoundResponse("Historical context not found");

            return ApiResponse<HistoricalContextResponse>.OkResponse(new HistoricalContextResponse(context), "Get historical context successfully", "200");
        }

        public async Task<ApiResponse<HistoricalContextResponse>> CreateAsync(HistoricalContextRequest request)
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();
            var newContext = new HistoricalContext
            {
                HistoricalContextId = Guid.NewGuid().ToString(),
                Title = request.Title,
                Period = request.Period,
                Description = request.Description,
                Status = request.Status
            };

            await repo.InsertAsync(newContext);

            var artifactRepo = _unitOfWork.GetRepository<ArtifactHistoricalContext>();
            foreach (var artifactId in request.ArtifactIds)
            {
                await artifactRepo.InsertAsync(new ArtifactHistoricalContext
                {
                    ArtifactId = artifactId,
                    HistoricalContextId = newContext.HistoricalContextId
                });
            }

            var exhibitionRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();
            foreach (var exhibitionId in request.ExhibitionIds)
            {
                await exhibitionRepo.InsertAsync(new ExhibitionHistoricalContext
                {
                    ExhibitionId = exhibitionId,
                    HistoricalContextId = newContext.HistoricalContextId
                });
            }

            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<HistoricalContextResponse>.OkResponse(
                new HistoricalContextResponse(newContext),
                "Created historical context successfully",
                "200"
            );
        }

        public async Task<ApiResponse<HistoricalContextResponse>> UpdateAsync(string id, HistoricalContextUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();
            var context = await repo.GetQueryable()
                .Include(x => x.ArtifactHistoricalContexts)
                .ThenInclude(x => x.Artifact)
                .Include(x => x.ExhibitionHistoricalContexts)
                .ThenInclude(x => x.Exhibition)
                .FirstOrDefaultAsync(x => x.HistoricalContextId == id);

            if (context == null)
                return ApiResponse<HistoricalContextResponse>.NotFoundResponse("Historical context not found");

            // Update fields
            context.Title = request.Title ?? context.Title;
            context.Period = request.Period ?? context.Period;
            context.Description = request.Description ?? context.Description;
            context.Status = request.Status ?? context.Status;

            await _unitOfWork.SaveChangeAsync();

            // Load lại quan hệ để trả về đầy đủ (nếu cần)
            var updated = await repo.GetQueryable()
                .Include(x => x.ArtifactHistoricalContexts)
                    .ThenInclude(x => x.Artifact)
                .Include(x => x.ExhibitionHistoricalContexts)
                    .ThenInclude(x => x.Exhibition)
                .FirstOrDefaultAsync(x => x.HistoricalContextId == id);

            return ApiResponse<HistoricalContextResponse>.OkResponse(
                new HistoricalContextResponse(updated),
                "Updated historical context successfully",
                "200"
            );
        }


        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();
            var context = await repo.GetByIdAsync(id);

            if (context == null)
                return ApiResponse<bool>.NotFoundResponse("Historical context not found");

            // ✅ Soft delete bằng cách cập nhật Status
            context.Status = "Deleted";

            // ✅ Nếu muốn, bạn có thể soft delete các liên kết liên quan
            var artifactRepo = _unitOfWork.GetRepository<ArtifactHistoricalContext>();
            var exhibitionRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();

            var artifactLinks = await artifactRepo.FilterByAsync(x => x.HistoricalContextId == id);
            var exhibitionLinks = await exhibitionRepo.FilterByAsync(x => x.HistoricalContextId == id);

            foreach (var link in artifactLinks)
            {
                // Nếu bảng link có cột Status thì set
                var statusProp = link.GetType().GetProperty("Status");
                if (statusProp != null)
                    statusProp.SetValue(link, "Deleted");
            }

            foreach (var link in exhibitionLinks)
            {
                var statusProp = link.GetType().GetProperty("Status");
                if (statusProp != null)
                    statusProp.SetValue(link, "Deleted");
            }

            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(true, "Soft deleted historical context successfully", "200");
        }




    }
}
