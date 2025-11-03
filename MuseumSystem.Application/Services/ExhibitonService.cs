using Microsoft.EntityFrameworkCore;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Utils;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class ExhibitionService : IExhibitionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ExhibitionService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        // 🔹 Lấy tất cả Exhibition có phân trang
        public async Task<ApiResponse<BasePaginatedList<ExhibitionResponse>>> GetAllAsync(
    int pageNumber = 1,
    int pageSize = 10,
    ExhibitionStatus? statusFilter = null)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();

            var query = repo.Entity
                .Include(e => e.ExhibitionHistoricalContexts)
                    .ThenInclude(eh => eh.HistoricalContext)
                .AsQueryable();

            // 🔍 Lọc theo trạng thái
            if (statusFilter.HasValue)
            {
                query = query.Where(e => e.Status == statusFilter.Value);
            }
            else
            {
                query = query.Where(e => e.Status != ExhibitionStatus.Deleted);
            }

            // 🔽 Sắp xếp theo Priority (0 là cao nhất), sau đó theo CreatedAt mới nhất
            query = query
                .OrderBy(e => e.Priority)
                .ThenByDescending(e => e.CreatedAt);

            var totalCount = await query.CountAsync();
            var exhibitions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = exhibitions.Select(e => new ExhibitionResponse(e)).ToList();
            var paginated = new BasePaginatedList<ExhibitionResponse>(result, totalCount, pageNumber, pageSize);

            return ApiResponse<BasePaginatedList<ExhibitionResponse>>.OkResponse(
                paginated,
                "Get exhibitions successfully",
                "200"
            );
        }




        // 🔹 Lấy Exhibition theo Id
        public async Task<ApiResponse<ExhibitionResponse>> GetByIdAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.Entity
                .Include(e => e.ExhibitionHistoricalContexts)
                    .ThenInclude(eh => eh.HistoricalContext)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exhibition == null)
                return ApiResponse<ExhibitionResponse>.NotFoundResponse("Exhibition not found");

            exhibition.Status = GetStatusByDate(exhibition.StartDate, exhibition.EndDate);

            return ApiResponse<ExhibitionResponse>.OkResponse(new ExhibitionResponse(exhibition), "Get exhibition successfully", "200");
        }

        // 🔹 Tạo mới Exhibition
        public async Task<ApiResponse<ExhibitionResponse>> CreateAsync(ExhibitionRequest request)
        {
            // Validate museum context
            var museumId = _currentUserService.MuseumId;
            if (string.IsNullOrEmpty(museumId))
                return ApiResponse<ExhibitionResponse>.BadRequestResponse("Cannot determine museum from current user context.");

            var now = DateTime.UtcNow;

            // Validate dates
            if (request.StartDate <= now)
                return ApiResponse<ExhibitionResponse>.BadRequestResponse("StartDate must be greater than current time.");
            if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
                return ApiResponse<ExhibitionResponse>.BadRequestResponse("EndDate must be greater than StartDate.");

            try
            {
                // Repos
                var exhibitionRepo = _unitOfWork.GetRepository<Exhibition>();
                var historicalRepo = _unitOfWork.GetRepository<HistoricalContext>();
                var linkRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();

                // 1) Create exhibition and save so it has an Id
                var newExhibition = new Exhibition
                {
                    Id = Guid.NewGuid().ToString(),    // your Exhibition.Id is a string
                    Name = request.Name?.Trim(),
                    Description = request.Description?.Trim(),
                    Priority = request.Priority,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    MuseumId = museumId,
                    CreatedAt = now,
                    Status = GetStatusByDate(request.StartDate, request.EndDate)
                };

                await exhibitionRepo.InsertAsync(newExhibition);
                await _unitOfWork.SaveChangeAsync(); // must save to ensure newExhibition.Id is persisted / stable

                // 2) If historicalContextIds provided, verify they exist first
                if (request.HistoricalContextIds != null && request.HistoricalContextIds.Any())
                {
                    var invalidIds = new List<string>();
                    var validIds = new List<string>();

                    foreach (var id in request.HistoricalContextIds.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        // Assumes repo exposes GetByIdAsync(string id). Adjust if method name is different.
                        var hc = await historicalRepo.GetByIdAsync(id);
                        if (hc == null)
                            invalidIds.Add(id);
                        else
                            validIds.Add(id);
                    }

                    if (invalidIds.Any())
                    {
                        // Rollback is optional here; HistoricalContext.CreateAsync does not use explicit transaction.
                        // But since exhibition already saved, return clear error so caller can fix IDs.
                        var msg = $"The following historicalContextIds do not exist: {string.Join(", ", invalidIds)}";
                        return ApiResponse<ExhibitionResponse>.BadRequestResponse(msg);
                    }

                    // 3) Insert link records for all valid ids
                    foreach (var historicalId in validIds)
                    {
                        var link = new ExhibitionHistoricalContext
                        {
                            ExhibitionId = newExhibition.Id,
                            HistoricalContextId = historicalId.Trim()
                        };

                        await linkRepo.InsertAsync(link);
                    }

                    await _unitOfWork.SaveChangeAsync();
                }

                // 4) Return success (no explicit transaction here - matching HistoricalContext flow)
                return ApiResponse<ExhibitionResponse>.OkResponse(
                    new ExhibitionResponse(newExhibition),
                    "Created exhibition successfully",
                    "200"
                );
            }
            catch (Exception ex)
            {
                // Return error with inner message for debugging (as HistoricalContext.CreateAsync does)
                return ApiResponse<ExhibitionResponse>.InternalErrorResponse(
                    $"Error: {ex.InnerException?.Message ?? ex.Message}"
                );
            }
        }






        // 🔹 Cập nhật Exhibition
        public async Task<ApiResponse<ExhibitionResponse>> UpdateAsync(string id, ExhibitionUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.GetByIdAsync(id);
            if (exhibition == null)
                return ApiResponse<ExhibitionResponse>.NotFoundResponse("Exhibition not found");

            var now = DateTime.UtcNow;

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate <= now)
                    return ApiResponse<ExhibitionResponse>.BadRequestResponse("StartDate must be in the future.");
                if (request.EndDate <= request.StartDate)
                    return ApiResponse<ExhibitionResponse>.BadRequestResponse("EndDate must be greater than StartDate.");
            }

            exhibition.Name = request.Name ?? exhibition.Name;
            exhibition.Description = request.Description ?? exhibition.Description;
            exhibition.Priority = request.Priority ?? exhibition.Priority;
            exhibition.StartDate = request.StartDate ?? exhibition.StartDate;
            exhibition.EndDate = request.EndDate ?? exhibition.EndDate;
            exhibition.Status = GetStatusByDate(exhibition.StartDate, exhibition.EndDate);
            exhibition.UpdatedAt = now;

            await repo.UpdateAsync(exhibition);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<ExhibitionResponse>.OkResponse(new ExhibitionResponse(exhibition), "Updated exhibition successfully", "200");
        }

        // 🔹 Xoá Exhibition
        /// <summary>
        /// 🔹 Soft delete Exhibition — chỉ đổi Status thành Deleted
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.GetByIdAsync(id);
            if (exhibition == null)
                return ApiResponse<bool>.NotFoundResponse("Exhibition not found");

            if (exhibition.Status == ExhibitionStatus.Deleted)
                return ApiResponse<bool>.BadRequestResponse("Exhibition is already deleted.");

            exhibition.Status = ExhibitionStatus.Deleted;
            exhibition.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(exhibition);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(true, "Soft deleted exhibition successfully", "200");
        }


        // 🔹 Gán thêm danh sách HistoricalContext vào Exhibition
        public async Task<ApiResponse<bool>> AssignHistoricalContextsAsync(string exhibitionId, ExhibitionHistoricalAssignRequest request)
        {
            var linkRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();
            var existingLinks = await linkRepo.GetAllAsync(q => q.Where(x => x.ExhibitionId == exhibitionId));

            var existingIds = existingLinks.Select(x => x.HistoricalContextId).ToList();
            var newIds = request.HistoricalContextIds.Except(existingIds).ToList();

            if (!newIds.Any())
                return ApiResponse<bool>.BadRequestResponse("No new historical contexts to assign.");

            foreach (var historicalId in newIds)
            {
                await linkRepo.InsertAsync(new ExhibitionHistoricalContext
                {
                    ExhibitionId = exhibitionId,
                    HistoricalContextId = historicalId
                });
            }

            await _unitOfWork.SaveChangeAsync();
            return ApiResponse<bool>.OkResponse(true, "Assigned historical contexts successfully.", "200");
        }

        // 🔹 Xoá một số HistoricalContext khỏi Exhibition
public async Task<ApiResponse<bool>> RemoveHistoricalContextsAsync(string exhibitionId, ExhibitionHistoricalAssignRequest request)
{
    try
    {
        var linkRepo = _unitOfWork.GetRepository<ExhibitionHistoricalContext>();

        // ✅ Lấy danh sách historical context cần xóa
        var linksToRemove = await linkRepo.GetQueryable()
            .Where(x => x.ExhibitionId == exhibitionId && request.HistoricalContextIds.Contains(x.HistoricalContextId))
            .ToListAsync();

        if (!linksToRemove.Any())
            return ApiResponse<bool>.BadRequestResponse("Không tìm thấy historical context nào để xóa.");

        await _unitOfWork.BeginTransactionAsync();

        // ✅ Xóa nhiều historical context cùng lúc bằng DeleteRangeAsync
        await linkRepo.DeleteRangeAsync(linksToRemove);

        await _unitOfWork.SaveChangeAsync();
        await _unitOfWork.CommitTransactionAsync();

        return ApiResponse<bool>.OkResponse(true, "Removed historical contexts successfully", "200");
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollBackAsync();
        return ApiResponse<bool>.BadRequestResponse($"Lỗi khi xóa historical contexts: {ex.Message}");
    }
}


        // 🔹 Xác định trạng thái theo ngày
        private ExhibitionStatus GetStatusByDate(DateTime? startDate, DateTime? endDate)
        {
            var now = DateTime.UtcNow;
            if (startDate == null || endDate == null)
                return ExhibitionStatus.Upcoming;

            if (now < startDate.Value)
                return ExhibitionStatus.Upcoming;

            if (now >= startDate.Value && now <= endDate.Value)
                return ExhibitionStatus.Active;

            return ExhibitionStatus.Expired;
        }
    }
}
