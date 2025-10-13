using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AreaDtos;
using MuseumSystem.Application.Interfaces;

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
                return Ok(ApiResponse<AreaResponse>.OkResponse(result, $"Create Area: '{result.Name}' sucessfully" , "200"));   
        }
    }
}
