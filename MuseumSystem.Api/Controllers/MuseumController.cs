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
            try
            {
                var muse = await service.CreateMuseum(museum);
                return Ok(ApiResponse<Museum>.OkResponse(muse,"Create museum successful!", "200"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding museum");
                return StatusCode(500, ApiResponse<AccountRespone>.InternalErrorResponse(ex.Message));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMuseums(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var museums = await service.GetAll(pageIndex, pageSize);
                return Ok(ApiResponse<BasePaginatedList<Museum>>.OkResponse(museums, "Get all museums successful!", "200"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all museums");
                return StatusCode(500, ApiResponse<AccountRespone>.InternalErrorResponse(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMuseum(string id)
        {
            try
            {
                await service.DeleteMuseum(id);
                return Ok(ApiResponse<string>.OkResponse(id, "Delete museum successful!", "200"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting museum");
                return StatusCode(500, ApiResponse<AccountRespone>.InternalErrorResponse(ex.Message));
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMuseumById(string id)
        {
            try
            {
                var museum = await service.GetMuseumById(id);
                return Ok(ApiResponse<Museum>.OkResponse(museum, "Get museum by id successful!", "200"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting museum by id");
                return StatusCode(500, ApiResponse<AccountRespone>.InternalErrorResponse(ex.Message));
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMuseum(string id, [FromBody] MuseumRequest museumRequest)
        {
            try
            {
                var updatedMuseum = await service.UpdateMuseum(id, museumRequest);
                return Ok(ApiResponse<Museum>.OkResponse(updatedMuseum, "Update museum successful!", "200"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating museum");
                return StatusCode(500, ApiResponse<AccountRespone>.InternalErrorResponse(ex.Message));
            }
        }


    }
}
