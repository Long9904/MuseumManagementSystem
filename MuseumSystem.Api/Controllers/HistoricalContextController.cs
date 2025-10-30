﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.HistoricalContexts;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MuseumSystem.API.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [SwaggerTag("Historical Context Manage - Admin,Staff")]
    public class HistoricalContextController : ControllerBase
    {
        private readonly IHistoricalContextService _historicalContextService;

        public HistoricalContextController(IHistoricalContextService historicalContextService)
        {
            _historicalContextService = historicalContextService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all historical contexts",
            Description = "Retrieves a list of all historical contexts available in the museum."
        )]
        public async Task<IActionResult> GetAll()
        {
            var response = await _historicalContextService.GetAllAsync();
            return StatusCode((int)response.Code, response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get historical context by ID",
            Description = "Retrieves detailed information about a specific historical context by its ID."
        )]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _historicalContextService.GetByIdAsync(id);
            return StatusCode((int)response.Code, response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new historical context",
            Description = "Creates a new historical context record with information such as title, description, and period."
        )]
        public async Task<IActionResult> Create([FromBody] HistoricalContextRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _historicalContextService.CreateAsync(request);
            return StatusCode((int)response.Code, response);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update historical context information",
            Description = "Updates details of an existing historical context identified by its ID."
        )]
        public async Task<IActionResult> Update(string id, [FromBody] HistoricalContextUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _historicalContextService.UpdateAsync(id, request);
            return StatusCode((int)response.Code, response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a historical context",
            Description = "Deletes a historical context record from the system using its unique ID."
        )]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _historicalContextService.DeleteAsync(id);
            return StatusCode((int)response.Code, response);
        }
    }
}
