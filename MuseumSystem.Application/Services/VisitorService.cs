using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Application.Dtos.HistoricalContextsDtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
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

        public async Task<BasePaginatedList<MyInteractionResponse>> MyInteractionsAsync(
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
            var result = _mapper.Map<BasePaginatedList<Interaction>, BasePaginatedList<MyInteractionResponse>>(paginatedInteractions);
            return result;
        }

        public async Task<BasePaginatedList<VisitorInteractionResponse>> GetAllInteractionsByArtifactAsync(
            string artifactId,
            int pageIndex,
            int pageSize)
        {
            var artifact = await _unitOfWork.GetRepository<Artifact>()
                .FindAsync(a => a.Id == artifactId
                && (a.Status == ArtifactStatus.OnDisplay || a.Status == ArtifactStatus.InStorage))
                ?? throw new NotFoundException("Artifact not found.");

            var query = _unitOfWork.GetRepository<Interaction>().Entity
                .Where(i => i.ArtifactId == artifactId)
                .Include(i => i.Visitor)
                .OrderByDescending(i => i.CreatedAt);

            BasePaginatedList<Interaction> paginatedInteractions =
                await _unitOfWork.GetRepository<Interaction>().GetPagging(query, pageIndex, pageSize);

            var result = _mapper.Map<BasePaginatedList<Interaction>, BasePaginatedList<VisitorInteractionResponse>>(paginatedInteractions);

            return result;
        }

        public async Task<MyInteractionResponse> PostInteractionAsync(InteractionRequest request)
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

            return new MyInteractionResponse
            {
                InteractionId = interaction.Id,
                ArtifactId = interaction.ArtifactId,
                Comment = interaction.Comment,
                Rating = interaction.Rating,
                CreatedAt = interaction.CreatedAt
            };
        }


        public async Task<BasePaginatedList<MuseumResponseV1>> GetMuseumsAsync(int pageIndex, int pageSize, string? museumName = null)
        {
            var query = _unitOfWork.GetRepository<Museum>().Entity
                .OrderBy(m => m.Name)
                .Where(m => m.Status == EnumStatus.Active);

            if (!string.IsNullOrEmpty(museumName))
            {
                query = query.Where(m => m.Name.ToLower().Contains(museumName));
            }

            BasePaginatedList<Museum> basePaginatedList =
            await _unitOfWork.GetRepository<Museum>().GetPagging(query, pageIndex, pageSize);

            var result = _mapper.Map<BasePaginatedList<Museum>, BasePaginatedList<MuseumResponseV1>>(basePaginatedList);
            return result;
        }

        public async Task<MuseumResponseV1> GetMuseumByIdAsync(string museumId)
        {
            var museum = await _unitOfWork.GetRepository<Museum>()
                .FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active);

            if (museum == null)
            {
                throw new NotFoundException("Museum not found.");
            }
            var result = _mapper.Map<Museum, MuseumResponseV1>(museum);
            return result;
        }

        public async Task<BasePaginatedList<ArtifactDetailsResponse>> GetAllArtifactsByMuseumAsync(
            string museumId,
            int pageIndex,
            int pageSize,
            string? artifactName = null,
            string? periodTime = null,
            string? areaName = null,
            string? displayPositionName = null)
        {
            // 1. Verify museum exists
            var museum = await _unitOfWork.GetRepository<Museum>()
                .FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException("Museum not found.");

            // 2. Build query for artifacts
            var query = _unitOfWork.GetRepository<Artifact>().Entity
                .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area)
                .Include(a => a.ArtifactMedias)
                .Include(a => a.Museum)
                .Where(a => a.MuseumId == museumId
                && (a.Status == ArtifactStatus.OnDisplay));

            if (!string.IsNullOrEmpty(artifactName))
            {
                query = query.Where(a => a.Name.ToLower().Contains(artifactName));
            }

            if (!string.IsNullOrEmpty(periodTime))
            {
                query = query.Where(a => a.PeriodTime != null && a.PeriodTime.ToLower().Contains(periodTime));
            }

            if (!string.IsNullOrEmpty(areaName))
            {
                query = query.Where(a => a.DisplayPosition != null
                    && a.DisplayPosition.Area != null
                    && a.DisplayPosition.Area.Name.ToLower().Contains(areaName));
            }

            if (!string.IsNullOrEmpty(displayPositionName))
            {
                query = query.Where(a => a.DisplayPosition != null
                    && a.DisplayPosition.DisplayPositionName.ToLower().Contains(displayPositionName));
            }

            query = query.OrderBy(a => a.Name);

            // 3. Get paginated list
            BasePaginatedList<Artifact> paginatedArtifacts =
                await _unitOfWork.GetRepository<Artifact>().GetPagging(query, pageIndex, pageSize);

            // 5. Map to response

            var result = _mapper.Map<BasePaginatedList<Artifact>, BasePaginatedList<ArtifactDetailsResponse>>(paginatedArtifacts);
            return result;
        }

        public async Task<ArtifactDetailsResponse> GetArtifactByIdAsync(string artifactId)
        {
            Artifact artifact = await _unitOfWork.ArtifactRepository.FindAsync(
                a => a.Id == artifactId
                && (a.Status == ArtifactStatus.OnDisplay || a.Status == ArtifactStatus.InStorage),
                include: source => source
                    .Include(a => a.Museum)
                    .Include(a => a.ArtifactMedias)
                    .Include(a => a.DisplayPosition).ThenInclude(dp => dp.Area))
                ?? throw new NotFoundException($"Artifact with ID '{artifactId}' not found.");

            // Take list of medias and sort by MediaType and CreatedAt
            artifact.ArtifactMedias = artifact.ArtifactMedias
                .OrderBy(am => am.MediaType)
                .ThenByDescending(am => am.CreatedAt)
                .ToList();

            // Map to Media response

            var mediaResponses = artifact.ArtifactMedias
                .Select(m => new MediaResponse
                {
                    Id = m.Id,
                    MediaType = m.MediaType,
                    FilePath = m.FilePath,
                    FileFormat = m.FileFormat,
                    Caption = m.Caption,
                    Status = m.Status,
                })
                .OrderBy(m => m.MediaType)
                .ThenByDescending(m => m.CreatedAt).ToList();

            var artifactDetailsResponse = _mapper.Map<ArtifactDetailsResponse>(artifact);
            artifactDetailsResponse.MediaItems = mediaResponses;
            return artifactDetailsResponse;
        }

        public async Task<BasePaginatedList<ExhibitionResponseV2>> GetAllExhibitions
            (int pageIndex, int pageSize, string? exhibitionName = null, string museumId = null!)
        {
            var query = _unitOfWork.GetRepository<Exhibition>().Entity
                .Include(e => e.Museum)
                .Where(e =>
                e.Status == ExhibitionStatus.Upcoming || e.Status == ExhibitionStatus.Active);

            if (!string.IsNullOrEmpty(exhibitionName))
            {
                query = query.Where(e => e.Name.ToLower().Contains(exhibitionName));
            }
            query = query.Where(e => e.MuseumId == museumId);

            query = query.OrderBy(e => e.StartDate);

            BasePaginatedList<Exhibition> basePaginatedList =
            await _unitOfWork.GetRepository<Exhibition>().GetPagging(query, pageIndex, pageSize);
            var result = _mapper.Map<BasePaginatedList<Exhibition>, BasePaginatedList<ExhibitionResponseV2>>(basePaginatedList);
            return result;
        }

        public async Task<ExhibitionResponseV2> GetByIdAsync(string exhibitionId)
        {
            var exhibition = await _unitOfWork.GetRepository<Exhibition>()
                .FindAsync(e => e.Id == exhibitionId
                && (e.Status == ExhibitionStatus.Upcoming || e.Status == ExhibitionStatus.Active),
                include: source => source
                    .Include(e => e.ExhibitionHistoricalContexts)
                        .ThenInclude(eh => eh.HistoricalContext)
                            .ThenInclude(hc => hc.ArtifactHistoricalContexts).ThenInclude(a => a.Artifact));

            if (exhibition == null)
            {
                throw new NotFoundException("Exhibition not found.");
            }

            List<HistoricalContextResponseV2> historicalContexts = new List<HistoricalContextResponseV2>();

            if (exhibition.ExhibitionHistoricalContexts != null)
            {
                // Take each HistoricalContext from ExhibitionHistoricalContexts
                foreach (var ehc in exhibition.ExhibitionHistoricalContexts)
                {
                    var hc = ehc.HistoricalContext;
                    if (hc != null)
                    {
                        var hcResponse = new HistoricalContextResponseV2 // List HistoricalContextResponse
                        {
                            Id = hc.HistoricalContextId,
                            Title = hc.Title,
                            Period = hc.Period,
                            Description = hc.Description,

                            // Map Artifacts in HistoricalContext
                            Artifacts = hc.ArtifactHistoricalContexts?.Select(ahc => new ListArtifactInHistoricalContext
                            {  // Map each ArtifactHistoricalContext to ListArtifactInHistoricalContext
                                ArtifactId = ahc.ArtifactId,
                                ArtifactName = ahc.Artifact.Name
                            }).ToList() ?? new List<ListArtifactInHistoricalContext>()
                        };
                        historicalContexts.Add(hcResponse);
                    }
                }
            }
            ExhibitionResponseV2 result = new ExhibitionResponseV2()
            {
                Id = exhibition.Id,
                Name = exhibition.Name,
                Description = exhibition.Description,
                Priority = exhibition.Priority,
                StartDate = exhibition.StartDate,
                EndDate = exhibition.EndDate,
                Status = exhibition.Status,
                HistoricalContexts = historicalContexts
            };

            return result;

        }
    }
}
