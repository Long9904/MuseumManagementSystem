using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MuseumSystem.API.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [SwaggerTag("Exhibition Manage - Admin,Staff")]
    public class ExhibitionController : ControllerBase
    {
        private readonly IExhibitionService _exhibitionService;

        public ExhibitionController(IExhibitionService exhibitionService)
        {
            _exhibitionService = exhibitionService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all exhibitions",
            Description = "Retrieves a list of all exhibitions available in the museum. "
        )]
        public async Task<IActionResult> GetAll()
        {
            var response = await _exhibitionService.GetAllAsync();
            return StatusCode((int)response.Code, response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get exhibition by ID",
            Description = "Retrieves detailed information about a specific exhibition by its ID."
        )]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _exhibitionService.GetByIdAsync(id);
            return StatusCode((int)response.Code, response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new exhibition",
            Description = "Creates a new exhibition record with details such as title, description, and schedule."
        )]
        public async Task<IActionResult> Create([FromBody] ExhibitionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _exhibitionService.CreateAsync(request);
            return StatusCode((int)response.Code, response);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update exhibition information",
            Description = "Updates the details of an existing exhibition identified by its ID."
        )]
        public async Task<IActionResult> Update(string id, [FromBody] ExhibitionUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _exhibitionService.UpdateAsync(id, request);
            return StatusCode((int)response.Code, response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an exhibition",
            Description = "Deletes an exhibition record from the system using its unique ID"
        )]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _exhibitionService.DeleteAsync(id);
            return StatusCode((int)response.Code, response);
        }
        [HttpPost("{id}/assign-historical")]
        [SwaggerOperation(Summary = "Assign historical contexts", Description = "Assigns a list of historical contexts to the exhibition.")]
        public async Task<IActionResult> AssignHistoricalContexts(string id, [FromBody] ExhibitionHistoricalAssignRequest request)
        {
            var response = await _exhibitionService.AssignHistoricalContextsAsync(id, request);
            return StatusCode((int)response.Code, response);
        }

        [HttpPost("{id}/remove-historical")]
        [SwaggerOperation(Summary = "Remove historical contexts", Description = "Removes a list of historical contexts from the exhibition.")]
        public async Task<IActionResult> RemoveHistoricalContexts(string id, [FromBody] ExhibitionHistoricalAssignRequest request)
        {
            var response = await _exhibitionService.RemoveHistoricalContextsAsync(id, request);
            return StatusCode((int)response.Code, response);
        }

    }
}
