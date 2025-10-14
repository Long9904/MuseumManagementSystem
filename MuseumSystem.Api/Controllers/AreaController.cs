using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Api.Controllers
{
    [Authorize]
    [Route("api/v1/areas")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaRequest request)
        {
            var result = await _areaService.CreateArea(request);
            return Ok(ApiResponse<AreaResponse>.OkResponse(result, $"Create Area: '{result.Name}' sucessfully", "200"));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea([FromRoute] string id, [FromBody] AreaRequest request)
        {
            var result = await _areaService.UpdateArea(id, request);
            return Ok(ApiResponse<AreaResponse>.OkResponse(result, $"Update Area: '{result.Name}' sucessfully", "200"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea([FromRoute] string id)
        {
            await _areaService.DeleteArea(id);
            return Ok(ApiResponse<string>.OkResponse(null, $"Delete Area: '{id}' sucessfully", "200"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeDeleted = false)
        {
            var result = await _areaService.GetAll(pageIndex, pageSize, includeDeleted);
            return Ok(ApiResponse<BasePaginatedList<AreaResponse>>.OkResponse(result, $"Fetch {pageSize} items sucessfully", "200"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById([FromRoute] string id, [FromQuery] bool includeDeleted = false)
        {
            var result = await _areaService.GetAreaById(id, includeDeleted);
            return Ok(ApiResponse<AreaResponse>.OkResponse(result, $"Fetch Area: '{result.Name}' sucessfully", "200"));
        }
    }
}
