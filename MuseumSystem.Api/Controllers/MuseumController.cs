using Microsoft.AspNetCore.Authorization;
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
    [SwaggerTag("Museum Manage ")]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumService service;

        public MuseumController(IMuseumService serviceM)
        {
            service = serviceM;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation (Summary = "Register a new museum")]      
        public async Task<IActionResult> AddMuseum([FromBody] MuseumRequest museum)
        {

            var muse = await service.RegisterMuseum(museum);
            return Ok(ApiResponse<Museum>.OkResponse(muse, "Register museum successful!", "200"));

        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [SwaggerOperation (Summary = "Get all museums - has paging")]       
        public async Task<IActionResult> GetAllMuseums(int pageIndex = 1, int pageSize = 10, [FromQuery] MuseumFilterDtos? dtos = null)
        {
            var museums = await service.GetAll(pageIndex, pageSize, dtos);
            return Ok(ApiResponse<BasePaginatedList<Museum>>.OkResponse(museums, "Get all museums successful!", "200"));

        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        [SwaggerOperation (Summary = "Delete museum by ID - soft delete")]       
        public async Task<IActionResult> DeleteMuseum(string id)
        {

            await service.DeleteMuseum(id);
            return Ok(ApiResponse<string>.OkResponse("Delete museum successful!", "200"));

        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("active/{id}")]
        [SwaggerOperation (Summary = "Activate museum by ID")]
        public async Task<IActionResult> ActiveMuseum(string id)
        {
            await service.ActiveMuseum(id);
            return Ok(ApiResponse<string>.OkResponse("Activate museum successful!", "200"));
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("{id}")]
        [SwaggerOperation (Summary = "Get museum by ID")]       
        public async Task<IActionResult> GetMuseumById(string id)
        {
            var museum = await service.GetMuseumById(id);
            return Ok(ApiResponse<Museum>.OkResponse(museum, "Get museum by id successful!", "200"));

        }
        [Authorize (Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        [SwaggerOperation (Summary = "Update museum by ID")]
        public async Task<IActionResult> UpdateMuseum(string id, [FromBody] MuseumRequest museumRequest)
        {
            var updatedMuseum = await service.UpdateMuseum(id, museumRequest);
            return Ok(ApiResponse<Museum>.OkResponse(updatedMuseum, "Update museum successful!", "200"));
        }
    }
}
