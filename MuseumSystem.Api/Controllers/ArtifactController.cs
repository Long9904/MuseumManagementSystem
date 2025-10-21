using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Authorize(Roles = "Staff,Admin,Manager")]
    [Route("api/v1/artifacts")]
    [ApiController]
    [SwaggerTag(" Artifact Management - Staff, Admin, Manager")]
    public class ArtifactController : ControllerBase
    {
        private readonly IArtifactService _artifactService;
        private readonly IArtifactMediaService _artifactMediaService;

        public ArtifactController(IArtifactService artifactService, IArtifactMediaService artifactMediaService)
        {
            _artifactService = artifactService;
            _artifactMediaService = artifactMediaService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new artifact")]
        public async Task<IActionResult> CreateArtifact([FromBody] ArtifactRequest request)
        {
            var result = await _artifactService.CreateArtifact(request);
            return Ok(ApiResponse<ArtifactResponse>.OkResponse(result, $"Create artifact: '{result.Name}' sucessfully", "200"));
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all artifacts with pagination and filtering")]
        public async Task<IActionResult> GetAllArtifacts(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? periodTime = null,
            [FromQuery] bool includeDeleted = false)
        {
            var result = await _artifactService.GetAllArtifacts(pageIndex, pageSize, name, periodTime, includeDeleted);
            return Ok(ApiResponse<BasePaginatedList<ArtifactResponse>>.OkResponse(result, "Get all artifacts successfully", "200"));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get an artifact by ID")]
        public async Task<IActionResult> GetArtifactById([FromRoute] string id, [FromQuery] bool includeDeleted = false)
        {
            var result = await _artifactService.GetArtifactById(id, includeDeleted);
            return Ok(ApiResponse<ArtifactDetailsResponse>.OkResponse(result, $"Get artifact: '{result.Name}' by Id sucessfully", "200"));
        }

        [HttpGet("code/{artifactCode}")]
        [SwaggerOperation(
            Summary = "Get an artifact by Code")]
        public async Task<IActionResult> GetArtifactByCode([FromRoute] string artifactCode)
        {
            var result = await _artifactService.GetArtifactByCode(artifactCode);
            return Ok(ApiResponse<ArtifactResponse>.OkResponse(result, $"Get artifact: '{result.Name}' by Code sucessfully", "200"));
        }

        [HttpPatch("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing artifact")]
        public async Task<IActionResult> UpdateArtifact([FromRoute] string id, [FromBody] ArtifactRequest request)
        {
            var result = await _artifactService.UpdateArtifact(id, request);
            return Ok(ApiResponse<ArtifactResponse>.OkResponse(result, $"Update artifact: '{result.Name}' sucessfully", "200"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an artifact - soft delete")]
        public async Task<IActionResult> DeleteArtifact([FromRoute] string id)
        {
            await _artifactService.DeleteArtifact(id);
            return Ok(ApiResponse<string>.OkResponse(null, $"Delete artifact: '{id}' sucessfully", "200"));
        }

        [HttpPatch("{id}/activate")]
        [SwaggerOperation(
            Summary = "Activate a soft-deleted artifact")]
        public async Task<IActionResult> ActivateArtifact([FromRoute] string id)
        {
            await _artifactService.ActiveArtifact(id);
            return Ok(ApiResponse<string>.OkResponse(null, $"Activate artifact: '{id}' sucessfully", "200"));
        }

        // Assign artifact to display position
        [HttpPatch("{artifactId}/assign-display-position/{displayPositionId}")]
        [SwaggerOperation(
            Summary = "Assign an artifact to a display position")]
        public async Task<IActionResult> AssignArtifactToDisplayPosition([FromRoute] string artifactId, [FromRoute] string displayPositionId)
        {
            var result = await _artifactService.AssignArtifactToDisplayPosition(artifactId, displayPositionId);
            return Ok(ApiResponse<ArtifactResponse>.OkResponse(result, $"Assign artifact: '{result.Name}' to display position '{result.DisplayPositionName}' sucessfully", "200"));
        }

        // Remove artifact from display position
        [HttpPatch("{artifactId}/remove-display-position")]
        [SwaggerOperation(
            Summary = "Remove an artifact from its display position")]
        public async Task<IActionResult> RemoveArtifactFromDisplayPosition([FromRoute] string artifactId)
        {
            var result = await _artifactService.RemoveArtifactFromDisplayPosition(artifactId);
            return Ok(ApiResponse<ArtifactResponse>.OkResponse(result, $"Remove artifact: '{result.Name}' from its display position sucessfully", "200"));
        }



        //--------Media Management for Artifact ---------//


        [HttpPost("{artifactId}/media")]
        [SwaggerOperation(
            Summary = "Add media to an artifact")]
        public async Task<IActionResult> AddArtifactMedia([FromRoute] string artifactId, [FromForm] MediaRequest mediaRequest)
        {
            var result = await _artifactMediaService.UploadArtifactMediaAsync(artifactId, mediaRequest);
            return Ok(ApiResponse<MediaResponse>.OkResponse(result, $"Upload media to artifact '{artifactId}' successfully", "200"));

        }


        [HttpPut("{artifactId}/media/{mediaId}")]
        [SwaggerOperation(
            Summary = "Update media of an artifact")]
        public async Task<IActionResult> UpdateArtifactMedia(
            [FromRoute] string artifactId, 
            [FromRoute] string mediaId, 
            [FromForm] MediaRequest mediaRequest)
        {
            var result = await _artifactMediaService.UpdateArtifactMediaAsync(artifactId, mediaId, mediaRequest);
            return Ok(ApiResponse<MediaResponse>.OkResponse(result, $"Update media '{mediaId}' of artifact '{artifactId}' successfully", "200"));
        }


        [HttpDelete("{artifactId}/media/{mediaId}")]
        [SwaggerOperation(
            Summary = "Soft delete media from an artifact")]
        public async Task<IActionResult> DeleteArtifactMedia([FromRoute] string artifactId, [FromRoute] string mediaId)
        {
            var result = await _artifactMediaService.DeleteArtifactMediaAsync(artifactId, mediaId);
            return Ok(ApiResponse<bool>.OkResponse(result, $"Delete media '{mediaId}' from artifact '{artifactId}' successfully", "200"));
        }

    }
}
