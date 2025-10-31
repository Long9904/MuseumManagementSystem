using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.VisitorDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/visitors")]
    [SwaggerTag("Visitor api")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorService _visitorService;

        public VisitorController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }


        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Visitor login",
            Description = "Authenticate a visitor using their credentials and obtain an access token.")]
        public async Task<IActionResult> LoginVisitorAsync([FromBody] VisitorRequest visitorRequest)
        {
            var result = await _visitorService.LoginVisitorAsync(visitorRequest);
            return Ok(ApiResponse<VisitorLoginResponse>.OkResponse(result, "Login successfully", "200"));
        }


        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new visitor",
            Description = "Create a new visitor account with the provided information.")]
        public async Task<IActionResult> RegisterVisitorAsync([FromBody] VisitorRequest visitorRequest)
        {
            var result = await _visitorService.RegisterVisitorAsync(visitorRequest);
            return Ok(ApiResponse<VisitorResponse>.OkResponse(result, "Register successfully", "200"));
        }


        [HttpGet("me")]
        [SwaggerOperation(
            Summary = "Get my profile information",
            Description = "Retrieve the profile information of the currently authenticated visitor.")]
        public async Task<IActionResult> MyProfileAsync()
        {
            var result = await _visitorService.MyProfileAsync();
            return Ok(ApiResponse<VisitorResponse>.OkResponse(result, "Take profile sucessfully", "200"));
        }


        [HttpPost("interactions")]
        [SwaggerOperation(
            Summary = "Post a visitor interaction - Visitor Access",
            Description = "Submit an interaction (e.g., comment, rating) as the currently authenticated visitor.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> PostInteractionAsync([FromBody] InteractionRequest request)
        {
            var result = await _visitorService.PostInteractionAsync(request);
            return Ok(ApiResponse<VisitorInteractionResponse>.OkResponse(result, "Interaction posted successfully", "200"));
        }


        [HttpGet("interactions")]
        [SwaggerOperation(
            Summary = "Get my interactions - Visitor Access",
            Description = "Retrieve a paginated list of interactions made by the currently authenticated visitor.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> MyInteractionsAsync(
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _visitorService.MyInteractionsAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<VisitorInteractionResponse>>.OkResponse(result, "Take interactions successfully", "200"));
        }
    }
}
