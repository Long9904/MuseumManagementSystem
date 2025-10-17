using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Services;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/museums")]
    [ApiController]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumService service;
        private readonly ILogger<MuseumController> logger;

        public MuseumController(IMuseumService service, ILogger<MuseumController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddMuseum([FromBody] MuseumRequest museum)
        {

            var muse = await service.CreateMuseum(museum);
            return Ok(ApiResponse<Museum>.OkResponse(muse, "Create museum successful!", "200"));

        }
        [HttpGet]
        public async Task<IActionResult> GetAllMuseums(int pageIndex = 1, int pageSize = 10, [FromQuery] MuseumFilterDtos? dtos = null)
        {
            var museums = await service.GetAll(pageIndex, pageSize, dtos);
            return Ok(ApiResponse<BasePaginatedList<Museum>>.OkResponse(museums, "Get all museums successful!", "200"));

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMuseum(string id)
        {

            await service.DeleteMuseum(id);
            return Ok(ApiResponse<string>.OkResponse(id, "Delete museum successful!", "200"));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMuseumById(string id)
        {
            var museum = await service.GetMuseumById(id);
            return Ok(ApiResponse<Museum>.OkResponse(museum, "Get museum by id successful!", "200"));

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMuseum(string id, [FromBody] MuseumRequest museumRequest)
        {
            var updatedMuseum = await service.UpdateMuseum(id, museumRequest);
            return Ok(ApiResponse<Museum>.OkResponse(updatedMuseum, "Update museum successful!", "200"));

        }
    }
}
