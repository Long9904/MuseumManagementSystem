using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateTokenService _generateTokenService;
        private readonly ILogger<VisitorService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public VisitorService(
            IUnitOfWork unitOfWork,
            IGenerateTokenService generateTokenService,
            ILogger<VisitorService> logger,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _generateTokenService = generateTokenService;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<VisitorLoginResponse> LoginVisitorAsync(VisitorRequest visitorRequest)
        {
            var existingVisitor = await _unitOfWork.GetRepository<Visitor>()
                .FindAsync(v => v.Username == visitorRequest.Username);

            if (existingVisitor == null)
            {
                _logger.LogWarning("Login attempt failed for username: {Username}", visitorRequest.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(visitorRequest.Password, existingVisitor.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (existingVisitor.Status != VisitorStatus.Active)
            {
                throw new UnauthorizedAccessException("Account is not active.");
            }

            var res = new VisitorLoginResponse
            {
                Token = _generateTokenService.GenerateVisitorToken(existingVisitor),
            };

            _logger.LogInformation("Visitor with username {Username} logged in successfully.", visitorRequest.Username);
            return res;
        }

        public async Task<VisitorResponse> MyProfileAsync()
        {
            var visitorId = _currentUserService.UserId;
            var visitor = await _unitOfWork.GetRepository<Visitor>().FindAsync(v => v.Id == visitorId);
            if (visitor == null)
            {
                throw new NotFoundException("Visitor not found.");
            }

            return new VisitorResponse
            {
                Id = visitor.Id,
                Username = visitor.Username,
                Status = visitor.Status,
                CreatedAt = visitor.CreatedAt,
                UpdatedAt = visitor.UpdatedAt
            };
        }


        public async Task<VisitorResponse> RegisterVisitorAsync(VisitorRequest visitorRequest)
        {
            var existingVisitor = await _unitOfWork.GetRepository<Visitor>()
                .FindAsync(v => v.Username == visitorRequest.Username);

            if (existingVisitor != null)
            {
                throw new ConflictException("Username already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(visitorRequest.Password);

            var newVisitor = new Visitor
            {
                Username = visitorRequest.Username,
                PasswordHash = hashedPassword,
                Status = VisitorStatus.Active,
            };

            await _unitOfWork.GetRepository<Visitor>().InsertAsync(newVisitor);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("New visitor registered with username: {Username}", visitorRequest.Username);
            return new VisitorResponse
            {
                Id = newVisitor.Id,
                Username = newVisitor.Username,
                Status = newVisitor.Status,
                CreatedAt = newVisitor.CreatedAt,
                UpdatedAt = newVisitor.UpdatedAt
            };
        }

        public async Task<BasePaginatedList<VisitorInteractionResponse>> MyInteractionsAsync(
            int pageIndex, int pageSize)
        {
            var visitorId = _currentUserService.UserId;
            // Get all interactions by visitor
            var query = _unitOfWork.GetRepository<Interaction>().Entity
                .Where(i => i.VisitorId == visitorId)
                .Include(i => i.Artifact).ThenInclude(a => a.Museum)
                .Include(i => i.Visitor)
                .OrderByDescending(i => i.CreatedAt);

            BasePaginatedList<Interaction> paginatedInteractions = await _unitOfWork.GetRepository<Interaction>().GetPagging(query, pageIndex, pageSize);

            // Map to response
            var result = _mapper.Map<BasePaginatedList<Interaction>, BasePaginatedList<VisitorInteractionResponse>>(paginatedInteractions);
            return result;
        }


        public async Task<VisitorInteractionResponse> PostInteractionAsync(InteractionRequest request)
        {
            var visitorId = _currentUserService.UserId;
            var visitor = await _unitOfWork.GetRepository<Visitor>().FindAsync(v => v.Id == visitorId);
            if (visitor == null)
            {
                throw new NotFoundException("Visitor not found.");
            }

            // Create new Interaction
            var interaction = new Interaction
            {
                VisitorId = visitor.Id,
                ArtifactId = request.ArtifactId,
                Comment = request.Comment,
                Rating = request.Rating
            };

            await _unitOfWork.GetRepository<Interaction>().InsertAsync(interaction);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Visitor {VisitorId} posted a new interaction on Artifact {ArtifactId}.", visitor.Id, request.ArtifactId);

            return new VisitorInteractionResponse
            {
                InteractionId = interaction.Id,
                ArtifactId = interaction.ArtifactId,
                Comment = interaction.Comment,
                Rating = interaction.Rating,
                CreatedAt = interaction.CreatedAt
            };
        }

    }
}
