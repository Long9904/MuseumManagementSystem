using Microsoft.EntityFrameworkCore;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.HistoricalContexts;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
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

        public async Task<ApiResponse<BasePaginatedList<HistoricalContextResponse>>> GetAllAsync(
    int pageIndex,
    int pageSize,
    string? title,
    HistoricalStatus? statusFilter
)
        {
            var repo = _unitOfWork.GetRepository<HistoricalContext>();

            var query = repo.Entity
                .Include(x => x.ArtifactHistoricalContexts)
                    .ThenInclude(x => x.Artifact)
                .Include(x => x.ExhibitionHistoricalContexts)
                    .ThenInclude(x => x.Exhibition)
                .AsQueryable();

            // 🔍 Lọc theo trạng thái
            if (statusFilter.HasValue)
            {
                query = query.Where(x => x.Status == statusFilter.Value);
            }
            else
            {
                query = query.Where(x => x.Status != HistoricalStatus.Deleted);
            }

            // 🔍 Lọc theo tiêu đề (title)
            if (!string.IsNullOrWhiteSpace(title))
            {
                var lowerTitle = title.Trim().ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(lowerTitle));
            }

            // 🔽 Sắp xếp theo Title (A-Z)
            query = query.OrderBy(x => x.Title);

            // 📄 Sử dụng paging chuẩn (giống Area)
            var paginatedContexts = await repo.GetPagging(query, pageIndex, pageSize);

            // 🧩 Map sang DTO
            var result = paginatedContexts.Items.Select(x => new HistoricalContextResponse(x)).ToList();

            // 🧾 Tạo danh sách phân trang
            var paginated = new BasePaginatedList<HistoricalContextResponse>(
                result,
                paginatedContexts.TotalItems,
                pageIndex,
                pageSize
            );

            // ✅ Trả về ApiResponse chuẩn
            return ApiResponse<BasePaginatedList<HistoricalContextResponse>>.OkResponse(
                paginated,
                "Get all historical contexts successfully",
                "200"
            );
        }


        public async Task<ApiResponse<HistoricalContextResponse>> GetByIdAsync(string id)
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

            return ApiResponse<HistoricalContextResponse>.OkResponse(
                new HistoricalContextResponse(context),
                "Get historical context successfully",
                "200"
            );
        }

        public async Task<ApiResponse<HistoricalContextResponse>> CreateAsync(HistoricalContextRequest request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<HistoricalContext>();
                var newContext = new HistoricalContext
                {
                    Title = request.Title?.Trim(),
                    Period = request.Period,
                    Description = request.Description,
                    Status = HistoricalStatus.Active
                };

                await repo.InsertAsync(newContext);
                await _unitOfWork.SaveChangeAsync(); // có Id rồi

                // Gán artifacts (nếu có)
                if (request.ArtifactIds?.Any() == true)
                {
                    var artifactRepo = _unitOfWork.GetRepository<ArtifactHistoricalContext>();
                    foreach (var id in request.ArtifactIds)
                    {
                        await artifactRepo.InsertAsync(new ArtifactHistoricalContext
                        {
                            ArtifactId = id,
                            HistoricalContextId = newContext.HistoricalContextId
                        });
                    }
                }

                // Gán exhibitions (nếu có)
                //if (request.ExhibitionIds?.Any() == true)
                //{
                //    var exhibitionRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();
                //    foreach (var id in request.ExhibitionIds)
                //    {
                //        await exhibitionRepo.InsertAsync(new ExhibitionHistoricalContext
                //        {
                //            ExhibitionId = id,
                //            HistoricalContextId = newContext.HistoricalContextId
                //        });
                //    }
                //}

                await _unitOfWork.SaveChangeAsync();

                // Lấy lại entity vừa tạo để trả về
                var created = await repo.GetByIdAsync(newContext.HistoricalContextId);

                return ApiResponse<HistoricalContextResponse>.OkResponse(
                    new HistoricalContextResponse(created),
                    "Created historical context successfully",
                    "200"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<HistoricalContextResponse>.InternalErrorResponse(
                    $"Error creating historical context: {ex.InnerException?.Message ?? ex.Message}"
                );
            }
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

            // ✅ Cập nhật các trường cho phép
            context.Title = request.Title ?? context.Title;
            context.Period = request.Period ?? context.Period;
            context.Description = request.Description ?? context.Description;

            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<HistoricalContextResponse>.OkResponse(
                new HistoricalContextResponse(context),
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

            // ✅ Soft delete
            context.Status = HistoricalStatus.Deleted;
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(true, "Soft deleted historical context successfully", "200");
        }

        public async Task<ApiResponse<bool>> AssignArtifactsAsync(string historicalContextId, HistoricalArtifactAssignRequest request)
        {
            var contextRepo = _unitOfWork.GetRepository<HistoricalContext>();
            var artifactLinkRepo = _unitOfWork.GetRepository<ArtifactHistoricalContext>();

            var context = await contextRepo.GetByIdAsync(historicalContextId);
            if (context == null)
                return ApiResponse<bool>.NotFoundResponse("Historical context not found");

            var existingLinks = await artifactLinkRepo.GetQueryable()
                .Where(x => x.HistoricalContextId == historicalContextId)
                .Select(x => x.ArtifactId)
                .ToListAsync();

            var newArtifactIds = request.ArtifactIds.Except(existingLinks).ToList();

            foreach (var id in newArtifactIds)
            {
                await artifactLinkRepo.InsertAsync(new ArtifactHistoricalContext
                {
                    HistoricalContextId = historicalContextId,
                    ArtifactId = id
                });
            }

            await _unitOfWork.SaveChangeAsync();
            return ApiResponse<bool>.OkResponse(true, "Assigned artifacts successfully", "200");
        }

        public async Task<ApiResponse<bool>> RemoveArtifactsAsync(string historicalContextId, HistoricalArtifactAssignRequest request)
        {
            try
            {
                var artifactLinkRepo = _unitOfWork.GetRepository<ArtifactHistoricalContext>();

                // Lấy danh sách artifacts cần xóa
                var linksToRemove = await artifactLinkRepo.GetQueryable()
                    .Where(x => x.HistoricalContextId == historicalContextId && request.ArtifactIds.Contains(x.ArtifactId))
                    .ToListAsync();

                if (!linksToRemove.Any())
                    return ApiResponse<bool>.BadRequestResponse("Không tìm thấy artifact nào để xóa.");

                await _unitOfWork.BeginTransactionAsync();

                // ✅ Xóa nhiều artifact cùng lúc bằng DeleteRangeAsync
                await artifactLinkRepo.DeleteRangeAsync(linksToRemove);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.OkResponse(true, "Removed artifacts successfully", "200");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                return ApiResponse<bool>.BadRequestResponse($"Lỗi khi xóa artifacts: {ex.Message}");
            }
        }

    }
}
