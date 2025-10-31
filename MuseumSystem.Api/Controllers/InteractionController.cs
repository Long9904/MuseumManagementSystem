using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [SwaggerTag("Interaction Manage")]
    public class InteractionController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all interactions and by user logined - Admin, Staff",
            Description = "Retrieves all interactions between visitors and artifacts, including visitor and artifact details."
        )]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllInteractions()
        {
            var response = await _interactionService.GetAllAsync();
            return StatusCode((int)response.Code, response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get interaction by ID and by user logined - Admin, Staff",
            Description = "Retrieves the details of a specific interaction by its ID, including visitor and artifact information."
        )]

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetInteractionById(string id)
        {
            var response = await _interactionService.GetByIdAsync(id);
            return StatusCode((int)response.Code, response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new interaction, sample login - No role",
            Description = "Creates a new interaction record between a visitor and an artifact, including comments, ratings, and interaction type."
        )]
        public async Task<IActionResult> CreateInteraction([FromBody] InteractionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _interactionService.CreateAsync(request);
            return StatusCode((int)response.Code, response);
        }

        //[HttpPut("{id}")]
        //[SwaggerOperation(
        //    Summary = "Update an existing interaction",
        //    Description = "Updates an existing interaction identified by its ID. Allows modifying interaction type, comment, and rating."
        //)]
        //public async Task<IActionResult> UpdateInteraction(string id, [FromBody] InteractionUpdateRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

        //    var response = await _interactionService.UpdateAsync(id, request);
        //    return StatusCode((int)response.Code, response);
        //}

        //[HttpDelete("{id}")]
        //[SwaggerOperation(
        //    Summary = "Delete an interaction",
        //    Description = "Deletes an existing interaction identified by its ID from the system."
        //)]
        //public async Task<IActionResult> DeleteInteraction(string id)
        //{
        //    var response = await _interactionService.DeleteAsync(id);
        //    return StatusCode((int)response.Code, response);
        //}
    }
}
