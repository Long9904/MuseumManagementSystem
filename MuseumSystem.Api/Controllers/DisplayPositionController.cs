using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.DisplayPositionDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/display-postions")]
    [ApiController]
    public class DisplayPositionController : ControllerBase
    {
        private readonly IDisplayPositionService _displayPositionService;

        public DisplayPositionController(IDisplayPositionService displayPositionService)
        {
            _displayPositionService = displayPositionService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new display postion",
            Description = "Creates a new display postion with the provided details.")]
        public async Task<IActionResult> CreateDisplayPosition([FromBody] DisplayPositionRequest request)
        {
            var result = await _displayPositionService.CreateDisplayPosition(request);
            return Ok(ApiResponse<DisplayPositionResponse>.OkResponse(result, $"Create display postion: '{result.DisplayPositionName}' sucessfully", "200"));
        }


        [HttpPatch("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing display postion",
            Description = "Updates the details of an existing display postion identified by its ID.")]
        public async Task<IActionResult> UpdateDisplayPosition([FromRoute] string id, [FromBody] DisplayPositionRequest request)
        {
            var result = await _displayPositionService.UpdateDisplayPosition(id, request);
            return Ok(ApiResponse<DisplayPositionResponse>.OkResponse(result, $"Update display postion: '{result.DisplayPositionName}' sucessfully", "200"));
        }


        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a display postion",
            Description = "Deletes an existing display postion identified by its ID.")]
        public async Task<IActionResult> DeleteDisplayPosition([FromRoute] string id)
        {
            await _displayPositionService.DeleteDisplayPosition(id);
            return Ok(ApiResponse<string>.OkResponse(null, $"Delete display postion: '{id}' sucessfully", "200"));
        }


        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get a display postion by ID",
            Description = "Retrieves the details of a specific display postion identified by its ID.")]
        public async Task<IActionResult> GetDisplayPositionById([FromRoute] string id)
        {
            var result = await _displayPositionService.GetDisplayPositionById(id);
            return Ok(ApiResponse<DisplayPositionResponse>.OkResponse(result, $"Get display postion: '{result.DisplayPositionName}' by Id sucessfully", "200"));
        }


        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all display postions",
            Description = "Retrieves a paginated list of all display postions, with optional inclusion of deleted items.")]
        public async Task<IActionResult> GetAllDisplayPositions(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? artifactName = null,
            [FromQuery] string? displayPositionName = null,
            [FromQuery] string? areaName = null,
            [FromQuery] bool includeDeleted = false)
        {
            var result = await _displayPositionService.GetAllDisplayPositions(
                pageIndex, pageSize, 
                artifactName, displayPositionName, 
                areaName, includeDeleted);
            return Ok(ApiResponse<BasePaginatedList<DisplayPositionResponse>>.OkResponse(result, $"Get all display postions sucessfully", "200"));
        }
    }
}
