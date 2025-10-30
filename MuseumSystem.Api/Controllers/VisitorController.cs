using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace MuseumSystem.Api.Controllers
{
    
    [Route("api/v1/visitors")]
    [ApiController]
    [SwaggerTag("Visitor Manage - Quản lý khách tham quan bảo tàng")]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorService _visitorService;

        public VisitorController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        //[HttpGet]
        //[SwaggerOperation(
        //    Summary = "Get all visitors",
        //    Description = "Retrieves a list of all visitors who have visited or interacted with the museum."
        //)]
        //[Authorize(Roles = "Admin,Staff")]
        //public async Task<IActionResult> GetAllVisitors()
        //{
        //    var response = await _visitorService.GetAllAsync();
        //    return StatusCode((int)response.Code, response);
        //}

        //[Authorize(Roles = "Admin,Staff")]
        //[HttpGet("{id}")]
        //[SwaggerOperation(
        //    Summary = "Get visitor by ID",
        //    Description = "Retrieves detailed information about a specific visitor identified by their ID."
        //)]
        //public async Task<IActionResult> GetVisitorById(string id)
        //{
        //    var response = await _visitorService.GetByIdAsync(id);
        //    return StatusCode((int)response.Code, response);
        //}

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new visitor",
            Description = "Creates a new visitor record with basic information such as name, phone number, and email."
        )]
        public async Task<IActionResult> CreateVisitor([FromBody] VisitorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

            var response = await _visitorService.CreateAsync(request);
            return StatusCode((int)response.Code, response);
        }

        //[Authorize(Roles = "Admin,Staff")]
        //[HttpPut("{id}")]
        //[SwaggerOperation(
        //    Summary = "Update visitor information",
        //    Description = "Updates the information of an existing visitor identified by their ID."
        //)]
        //public async Task<IActionResult> UpdateVisitor(string id, [FromBody] VisitorUpdateRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid data."));

        //    var response = await _visitorService.UpdateAsync(id, request);
        //    return StatusCode((int)response.Code, response);
        //}

        //[Authorize(Roles = "Admin,Staff")]
        //[HttpDelete("{id}")]
        //[SwaggerOperation(
        //    Summary = "Delete a visitor",
        //    Description = "Deletes a visitor record from the system using their unique ID."
        //)]
        //public async Task<IActionResult> DeleteVisitor(string id)
        //{
        //    var response = await _visitorService.DeleteAsync(id);
        //    return StatusCode((int)response.Code, response);
        //}
    }
}
