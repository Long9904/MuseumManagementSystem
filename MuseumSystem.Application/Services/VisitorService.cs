using AutoMapper;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using MuseumSystem.Domain.Enums.EnumConfig;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VisitorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<VisitorResponse>>> GetAllAsync()
        {
            var visitors = await _unitOfWork.GetRepository<Visitor>().GetAllAsync();
            var data = _mapper.Map<List<VisitorResponse>>(visitors);

            return ApiResponse<List<VisitorResponse>>.OkResponse(
                data,
                "Get all visitors successfully",
                StatusCodeHelper.OK.Names()
            );
        }

        public async Task<ApiResponse<VisitorResponse>> GetByIdAsync(string id)
        {
            var visitor = await _unitOfWork.GetRepository<Visitor>().GetByIdAsync(id);
            if (visitor == null)
                return ApiResponse<VisitorResponse>.NotFoundResponse("Visitor not found");

            var data = _mapper.Map<VisitorResponse>(visitor);
            return ApiResponse<VisitorResponse>.OkResponse(
                data,
                "Get visitor successfully",
                StatusCodeHelper.OK.Names()
            );
        }

        public async Task<ApiResponse<VisitorResponse>> CreateAsync(VisitorRequest request)
        {
            var repo = _unitOfWork.GetRepository<Visitor>();

            // ✅ Check trùng số điện thoại
            var existing = await repo.FindByConditionAsync(v => v.PhoneNumber == request.PhoneNumber);
            if (existing != null)
            {
                return ApiResponse<VisitorResponse>.BadRequestResponse("Phone number already exists.");
            }

            // ✅ Map request -> entity
            var visitor = new Visitor
            {
                PhoneNumber = request.PhoneNumber,
                Status = EnumStatus.Active // ✅ Mặc định là Active
            };

            await repo.InsertAsync(visitor);
            await _unitOfWork.SaveChangeAsync();

            var data = _mapper.Map<VisitorResponse>(visitor);
            return new ApiResponse<VisitorResponse>(
                StatusCodeHelper.Created,
                StatusCodeHelper.Created.Names(),
                data,
                "Visitor created successfully."
            );
        }



        public async Task<ApiResponse<VisitorResponse>> UpdateAsync(string id, VisitorUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<Visitor>();
            var visitor = await repo.GetByIdAsync(id);

            if (visitor == null)
                return ApiResponse<VisitorResponse>.NotFoundResponse("Visitor not found");

            // ✅ Nếu có truyền Status thì convert string -> EnumStatus
            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<EnumStatus>(request.Status, true, out var parsedStatus))
                {
                    visitor.Status = parsedStatus;
                }
                else
                {
                    return ApiResponse<VisitorResponse>.BadRequestResponse("Invalid status value. Allowed values: Active, Inactive.");
                }
            }

            // ⚠️ Nếu bạn KHÔNG muốn cho phép đổi số điện thoại, có thể bỏ phần này
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                // Kiểm tra số điện thoại trùng
                var exists = await repo.FindByConditionAsync(v => v.PhoneNumber == request.PhoneNumber && v.Id != id);
                if (exists != null)
                {
                    return new ApiResponse<VisitorResponse>(
                        StatusCodeHelper.Conflict,
                        StatusCodeHelper.Conflict.Names(),
                        null,
                        "Phone number already exists."
                    );
                }

                visitor.PhoneNumber = request.PhoneNumber;
            }

            await repo.UpdateAsync(visitor);
            await _unitOfWork.SaveChangeAsync();

            var data = _mapper.Map<VisitorResponse>(visitor);
            data.Status = visitor.Status.ToString();

            return new ApiResponse<VisitorResponse>(
                StatusCodeHelper.OK,
                StatusCodeHelper.OK.Names(),
                data,
                "Visitor updated successfully"
            );
        }


        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Visitor>();
            var visitor = await repo.GetByIdAsync(id);
            if (visitor == null)
                return ApiResponse<bool>.NotFoundResponse("Visitor not found");

            await repo.DeleteAsync(id);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<bool>.OkResponse(
                true,
                "Visitor deleted successfully",
                StatusCodeHelper.OK.Names()
            );
        }
    }
}
