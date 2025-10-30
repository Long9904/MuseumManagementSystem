using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
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
    public class ExhibitionService : IExhibitionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExhibitionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy tất cả Exhibition
        /// </summary>
        public async Task<ApiResponse<List<ExhibitionResponse>>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibitions = await repo.GetAllAsync();
            var result = exhibitions.Select(e => new ExhibitionResponse(e)).ToList();

            return ApiResponse<List<ExhibitionResponse>>.OkResponse(result, "Get all exhibitions successfully", "200");
        }

        /// <summary>
        /// Lấy Exhibition theo Id
        /// </summary>
        public async Task<ApiResponse<ExhibitionResponse>> GetByIdAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.GetByIdAsync(id);

            if (exhibition == null)
                return ApiResponse<ExhibitionResponse>.NotFoundResponse("Exhibition not found");

            return ApiResponse<ExhibitionResponse>.OkResponse(new ExhibitionResponse(exhibition), "Get exhibition successfully", "200");
        }

        /// <summary>
        /// Tạo mới một Exhibition với logic auto status
        /// </summary>
        public async Task<ApiResponse<ExhibitionResponse>> CreateAsync(ExhibitionRequest request)
        {
            var newExhibition = new Exhibition
            {
                Name = request.Name,
                Description = request.Description,
                Priority = request.Priority,
                Status = request.Status,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MuseumId = request.MuseumId,
                CreatedAt = DateTime.UtcNow
            };

            await HandleExhibitionStatusRulesAsync(newExhibition);

            var repo = _unitOfWork.GetRepository<Exhibition>();
            await repo.InsertAsync(newExhibition);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<ExhibitionResponse>.OkResponse(
                new ExhibitionResponse(newExhibition),
                "Created exhibition successfully",
                "200"
            );
        }

        /// <summary>
        /// Cập nhật Exhibition
        /// </summary>
        public async Task<ApiResponse<ExhibitionResponse>> UpdateAsync(string id, ExhibitionUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.GetByIdAsync(id);
            if (exhibition == null)
                return ApiResponse<ExhibitionResponse>.NotFoundResponse("Exhibition not found");

            exhibition.Name = request.Name ?? exhibition.Name;
            exhibition.Description = request.Description ?? exhibition.Description;
            exhibition.Priority = request.Priority ?? exhibition.Priority;
            exhibition.Status = request.Status ?? exhibition.Status;
            exhibition.StartDate = request.StartDate ?? exhibition.StartDate;
            exhibition.EndDate = request.EndDate ?? exhibition.EndDate;
            exhibition.UpdatedAt = DateTime.UtcNow;

            await HandleExhibitionStatusRulesAsync(exhibition);

            await repo.UpdateAsync(exhibition);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<ExhibitionResponse>.OkResponse(
                new ExhibitionResponse(exhibition),
                "Updated exhibition successfully",
                "200"
            );
        }

        /// <summary>
        /// Xoá Exhibition
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var exhibition = await repo.GetByIdAsync(id);
            if (exhibition == null)
                return ApiResponse<bool>.NotFoundResponse("Exhibition not found");

            await repo.DeleteAsync(exhibition);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(true, "Deleted exhibition successfully", "200");
        }

        /// <summary>
        /// Logic quy tắc trạng thái tự động (Active/Expired)
        /// </summary>
        private async Task HandleExhibitionStatusRulesAsync(Exhibition exhibition)
        {
            var repo = _unitOfWork.GetRepository<Exhibition>();
            var allExhibitions = await repo.GetAllAsync();

            if (exhibition.Status == ExhibitionStatus.Active)
            {
                foreach (var ex in allExhibitions)
                {
                    if (ex.Id == exhibition.Id) continue;

                    if (ex.Priority != exhibition.Priority && ex.Priority != 0)
                    {
                        ex.Status = ExhibitionStatus.Expired;
                        await repo.UpdateAsync(ex);
                    }
                    else if (ex.Priority == exhibition.Priority && ex.Id != exhibition.Id)
                    {
                        ex.Status = ExhibitionStatus.Active;
                        await repo.UpdateAsync(ex);
                    }
                }
            }
        }
    }
}
