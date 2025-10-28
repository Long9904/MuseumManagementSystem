using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Services
{
    public class ArtifactMediaService : IArtifactMediaService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly CloudStorageService _storageService;
        private readonly ILogger<ArtifactMediaService> _logger;

        public ArtifactMediaService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            CloudStorageService storageService,
            ILogger<ArtifactMediaService> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<MediaResponse> UploadArtifactMediaAsync(string artifactId, MediaRequest mediaRequest)
        {
            Artifact artifact = await ValidateArtifactAccess(artifactId);

            var mediaUrl = string.Empty;
            // Upload media to cloud storage
            using var stream = mediaRequest.File.OpenReadStream();
            try
            {
                mediaUrl = await _storageService.Upload3DModelAsync(
                    stream, mediaRequest.File.FileName, mediaRequest.File.ContentType);
                if (string.IsNullOrEmpty(mediaUrl))
                {
                    throw new FileSaveException("Failed to upload media file URL.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media file for artifact {ArtifactId}", artifactId);
                throw new FileSaveException("Failed to upload media file.", ex);
            }

            ArtifactMediaType type = ArtifactMediaType.Image;

            if (mediaRequest.File.ContentType == "model/gltf+json" || mediaRequest.File.ContentType == "model/gltf-binary")
            {
                type = ArtifactMediaType.Model3D;
            }
            else if (mediaRequest.File.ContentType.StartsWith("image/"))
            {
                type = ArtifactMediaType.Image;
            }
            else
            {
                throw new InvalidFileTypeException("Unsupported media file type. Image: image/*, 3D Model: model/gltf+json, model/gltf-binary are supported.");
            }

            // Create ArtifactMedia entity
            var artifactMedia = new ArtifactMedia
            {
                MediaType = type,
                FilePath = mediaUrl,
                FileName = mediaRequest.File.FileName,
                MimeType = mediaRequest.File.ContentType,
                FileFormat = mediaRequest.File.FileName.Split('.').LastOrDefault(),
                Caption = mediaRequest.Caption,
                ArtifactId = artifact.Id,
                Artifact = artifact,
            };
            await _unitOfWork.GetRepository<ArtifactMedia>().InsertAsync(artifactMedia);
            await _unitOfWork.SaveChangeAsync();

            // Map to MediaResponse DTO
            var mediaResponse = new MediaResponse
            {
                Id = artifactMedia.Id,
                MediaType = artifactMedia.MediaType,
                FilePath = artifactMedia.FilePath,
                FileName = artifactMedia.FileName,
                MimeType = artifactMedia.MimeType,
                FileFormat = artifactMedia.FileFormat,
                Caption = artifactMedia.Caption,
                Status = artifactMedia.Status,
                CreatedAt = artifactMedia.CreatedAt,
                UpdatedAt = artifactMedia.UpdatedAt
            };
            return mediaResponse;
        }

        public async Task<MediaResponse> UpdateArtifactMediaAsync(string artifactId, string mediaId, MediaRequest mediaRequest)
        {
            Artifact artifact = await ValidateArtifactAccess(artifactId);

            ArtifactMedia artifactMedia = await _unitOfWork.GetRepository<ArtifactMedia>().GetByIdAsync(mediaId)
                ?? throw new NotFoundException($"Artifact media with ID '{mediaId}' not found.");

            if (artifactMedia.ArtifactId != artifact.Id)
            {
                throw new InvalidAccessException("User does not have access to this artifact media.");
            }

            var mediaUrl = string.Empty;
            string oldFilePath = artifactMedia.FilePath;
            // Upload new media to cloud storage if a new file is provided and remove the old one
            using var stream = mediaRequest.File.OpenReadStream();
            try
            {
                mediaUrl = await _storageService.Upload3DModelAsync(stream, mediaRequest.File.FileName, mediaRequest.File.ContentType);
                if (string.IsNullOrEmpty(mediaUrl))
                {
                    throw new FileSaveException("Meida url fail");
                }              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media file for artifact {ArtifactId}", artifactId);
                throw new FileSaveException("Failed to upload media file." , ex);
            }
            // Update caption if provided
            artifactMedia.FilePath = mediaUrl ?? artifactMedia.FilePath;
            artifactMedia.Caption = mediaRequest.Caption ?? artifactMedia.Caption;
            artifactMedia.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<ArtifactMedia>().UpdateAsync(artifactMedia);
            await _unitOfWork.SaveChangeAsync();

            // Delete old media from storage after successful save to db
            await _storageService.DeleteFileAsync(oldFilePath);

            // Map to MediaResponse DTO
            var mediaResponse = new MediaResponse
            {
                Id = artifactMedia.Id,
                MediaType = artifactMedia.MediaType,
                FilePath = artifactMedia.FilePath,
                FileName = artifactMedia.FileName,
                MimeType = artifactMedia.MimeType,
                FileFormat = artifactMedia.FileFormat,
                Caption = artifactMedia.Caption,
                Status = artifactMedia.Status,
                CreatedAt = artifactMedia.CreatedAt,
                UpdatedAt = artifactMedia.UpdatedAt
            };
            return mediaResponse;

        }

        public async Task<bool> DeleteArtifactMediaAsync(string artifactId, string mediaId)
        {
            Artifact artifact = await ValidateArtifactAccess(artifactId);

            ArtifactMedia artifactMedia = await _unitOfWork.GetRepository<ArtifactMedia>().GetByIdAsync(mediaId)
                ?? throw new NotFoundException($"Artifact media with ID '{mediaId}' not found.");

            if (artifact.Id != artifactMedia.ArtifactId)
            {
                throw new InvalidAccessException("User does not have access to this artifact media.");
            }

            // Change status to Deleted
            artifactMedia.Status = ArtifactMediaStatus.Deleted;
            artifactMedia.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<ArtifactMedia>().UpdateAsync(artifactMedia);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        private async Task<Artifact> ValidateArtifactAccess(string artifactId)
        {
            var museumId = await GetValidMuseumIdAsync();

            Artifact artifact = await _unitOfWork.ArtifactRepository.GetByIdAsync(artifactId)
                ?? throw new NotFoundException($"Artifact with ID '{artifactId}' not found.");

            if (artifact.MuseumId != museumId)
            {
                throw new InvalidAccessException("User does not have access to this artifact.");
            }

            if (artifact.Status == ArtifactStatus.Deleted)
            {
                throw new ObjectDeletedException("Cannot access a deleted artifact.");
            }

            return artifact;
        }


        private async Task<string> GetValidMuseumIdAsync()
        {
            var museumId = _currentUserService.MuseumId
                ?? throw new InvalidAccessException("User is not associated with any museum.");

            var museum = await _unitOfWork.MuseumRepository.FindAsync(m => m.Id == museumId && m.Status == EnumStatus.Active)
                ?? throw new NotFoundException($"Museum not found or status is not Active");

            return museumId;
        }
    }
}
