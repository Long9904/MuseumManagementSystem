using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/museums")]
    [ApiController]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumService service;

        public MuseumController(IMuseumService serviceM)
        {
            service = serviceM;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new museum",
            Description = "Creates a new museum")]
        public async Task<IActionResult> AddMuseum([FromBody] MuseumRequest museum)
        {

            var muse = await service.CreateMuseum(museum);
            return Ok(ApiResponse<Museum>.OkResponse(muse, "Create museum successful!", "200"));

        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all museums",
            Description = "Retrieves a paginated list of all museums.")]
        public async Task<IActionResult> GetAllMuseums(int pageIndex = 1, int pageSize = 10, [FromQuery] MuseumFilterDtos? dtos = null)
        {
            var museums = await service.GetAll(pageIndex, pageSize, dtos);
            return Ok(ApiResponse<BasePaginatedList<Museum>>.OkResponse(museums, "Get all museums successful!", "200"));

        }
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a museum",
            Description = "Deletes an existing museum identified by its ID.")]
        public async Task<IActionResult> DeleteMuseum(string id)
        {

            await service.DeleteMuseum(id);
            return Ok(ApiResponse<string>.OkResponse(id, "Delete museum successful!", "200"));

        }
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get a museum by ID",
            Description = "Retrieves the details of a specific museum identified by its ID.")]
        public async Task<IActionResult> GetMuseumById(string id)
        {
            var museum = await service.GetMuseumById(id);
            return Ok(ApiResponse<Museum>.OkResponse(museum, "Get museum by id successful!", "200"));

        }
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update a museum",
            Description = "Updates the details of an existing museum identified by its ID.")]
        public async Task<IActionResult> UpdateMuseum(string id, [FromBody] MuseumRequest museumRequest)
        {
            var updatedMuseum = await service.UpdateMuseum(id, museumRequest);
            return Ok(ApiResponse<Museum>.OkResponse(updatedMuseum, "Update museum successful!", "200"));

        }
    }
}
