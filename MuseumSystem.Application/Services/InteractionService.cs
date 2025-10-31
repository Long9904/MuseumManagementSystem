using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums.EnumConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public InteractionService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }



        // ✅ Lấy tất cả Interaction (có include Visitor + Artifact)
        public async Task<ApiResponse<List<InteractionResponse>>> GetAllAsync()
        {
            var repo = _unitOfWork.GetRepository<Interaction>();

            var interactions = await repo.Entity
                .Include(i => i.Visitor)
                .Include(i => i.Artifact)
                .Where(i => i.Artifact.MuseumId == _currentUserService.MuseumId)
                .ToListAsync();

            var data = interactions.Select(i => new InteractionResponse
            {
                Id = i.Id,
                VisitorId = i.VisitorId,
                ArtifactId = i.ArtifactId,
                Comment = i.Comment,
                Rating = i.Rating,
                CreatedAt = i.CreatedAt,
                VisitorPhoneNumber = i.Visitor?.PhoneNumber,
                ArtifactName = i.Artifact?.Name,
                ArtifactCode = i.Artifact?.ArtifactCode
            }).ToList();

            return ApiResponse<List<InteractionResponse>>.OkResponse(
                data,
                "Get all interactions successfully",
                StatusCodeHelper.OK.Names()
            );
        }

        // ✅ Lấy chi tiết Interaction
        public async Task<ApiResponse<InteractionResponse>> GetByIdAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Interaction>();

            var interaction = await repo.Entity
                .Include(i => i.Visitor)
                .Include(i => i.Artifact)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (interaction == null)
                return ApiResponse<InteractionResponse>.NotFoundResponse("Interaction not found");

            var data = new InteractionResponse
            {
                Id = interaction.Id,
                VisitorId = interaction.VisitorId,
                ArtifactId = interaction.ArtifactId,
                Comment = interaction.Comment,
                Rating = interaction.Rating,
                CreatedAt = interaction.CreatedAt,
                VisitorPhoneNumber = interaction.Visitor?.PhoneNumber,
                ArtifactName = interaction.Artifact?.Name,
                ArtifactCode = interaction.Artifact?.ArtifactCode
            };

            return ApiResponse<InteractionResponse>.OkResponse(
                data,
                "Get interaction successfully",
                StatusCodeHelper.OK.Names()
            );
        }

        // ✅ Tạo mới Interaction
        public async Task<ApiResponse<InteractionResponse>> CreateAsync(InteractionRequest request)
        {
            try
            {
                var visitor = await _unitOfWork.GetRepository<Visitor>().GetByIdAsync(request.VisitorId);
                if (visitor == null)
                    return ApiResponse<InteractionResponse>.BadRequestResponse("VisitorId không tồn tại trong hệ thống.");

                var artifact = await _unitOfWork.GetRepository<Artifact>().GetByIdAsync(request.ArtifactId);
                if (artifact == null)
                    return ApiResponse<InteractionResponse>.BadRequestResponse("ArtifactId không tồn tại trong hệ thống.");

                var interaction = _mapper.Map<Interaction>(request);
                interaction.Visitor = visitor;
                interaction.Artifact = artifact;

                await _unitOfWork.GetRepository<Interaction>().InsertAsync(interaction);
                await _unitOfWork.SaveChangeAsync();

                var data = new InteractionResponse
                {
                    Id = interaction.Id,
                    VisitorId = interaction.VisitorId,
                    ArtifactId = interaction.ArtifactId,
                    Comment = interaction.Comment,
                    Rating = interaction.Rating,
                    CreatedAt = interaction.CreatedAt,
                    VisitorPhoneNumber = visitor?.PhoneNumber,
                    ArtifactName = artifact?.Name,
                    ArtifactCode = artifact?.ArtifactCode
                };

                return new ApiResponse<InteractionResponse>(
                    StatusCodeHelper.Created,
                    StatusCodeHelper.Created.Names(),
                    data,
                    "Interaction created successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<InteractionResponse>.InternalErrorResponse(
                    $"Không thể tạo Interaction: {ex.InnerException?.Message ?? ex.Message}"
                );
            }
        }

        // ✅ Cập nhật Interaction
        public async Task<ApiResponse<InteractionResponse>> UpdateAsync(string id, InteractionUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<Interaction>();
            var interaction = await repo.Entity
                .Include(i => i.Visitor)
                .Include(i => i.Artifact)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (interaction == null)
                return ApiResponse<InteractionResponse>.NotFoundResponse("Interaction not found");

            if (request.Comment != null)
                interaction.Comment = request.Comment;

            if (request.Rating.HasValue)
                interaction.Rating = request.Rating.Value;

            await repo.UpdateAsync(interaction);
            await _unitOfWork.SaveChangeAsync();

            var data = new InteractionResponse
            {
                Id = interaction.Id,
                VisitorId = interaction.VisitorId,
                ArtifactId = interaction.ArtifactId,
                Comment = interaction.Comment,
                Rating = interaction.Rating,
                CreatedAt = interaction.CreatedAt,
                VisitorPhoneNumber = interaction.Visitor?.PhoneNumber,
                ArtifactName = interaction.Artifact?.Name,
                ArtifactCode = interaction.Artifact?.ArtifactCode
            };

            return ApiResponse<InteractionResponse>.OkResponse(
                data,
                "Interaction updated successfully",
                StatusCodeHelper.OK.Names()
            );
        }

        // ✅ Xóa Interaction
        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Interaction>();
            var interaction = await repo.GetByIdAsync(id);

            if (interaction == null)
                return ApiResponse<bool>.NotFoundResponse("Interaction not found");

            await repo.DeleteAsync(id);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(
                true,
                "Interaction deleted successfully",
                StatusCodeHelper.OK.Names()
            );
        }
    }
}
