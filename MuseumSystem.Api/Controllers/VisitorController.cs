using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Application.Dtos.ExhibitionDtos;
using MuseumSystem.Application.Dtos.InteractionDtos;
using MuseumSystem.Application.Dtos.MuseumDtos;
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
        [Authorize(Roles = "Visitor")]
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
            return Ok(ApiResponse<MyInteractionResponse>.OkResponse(result, "Interaction posted successfully", "200"));
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
            return Ok(ApiResponse<BasePaginatedList<MyInteractionResponse>>.OkResponse(result, "Take interactions successfully", "200"));
        }

        [HttpGet("artifacts/{artifactId}/interactions")]
        [SwaggerOperation(
            Summary = "Get all interactions for an artifact - Visitor Access",
            Description = "Retrieve a paginated list of all interactions associated with a specific artifact.")]

        public async Task<IActionResult> GetAllInteractionsByArtifactAsync(
            [FromRoute] string artifactId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _visitorService.GetAllInteractionsByArtifactAsync(artifactId, pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<VisitorInteractionResponse>>.OkResponse(result, "Take interactions successfully", "200"));
        }

        // Museum Enpoits for Visitor
        [HttpGet("museums")]
        [SwaggerOperation(
            Summary = "Get list of museums",
            Description = "Retrieve a paginated list of all museums available to visitors.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetMuseumsAsync(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            string? museumName = null)
        {
            var result = await _visitorService.GetMuseumsAsync(pageIndex, pageSize, museumName);
            return Ok(ApiResponse<BasePaginatedList<MuseumResponseV1>>.OkResponse(result, "Take museums successfully", "200"));
        }


        [HttpGet("museums/{museumId}")]
        [SwaggerOperation(
            Summary = "Get museum details by ID",
            Description = "Retrieve detailed information about a specific museum using its ID.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetMuseumByIdAsync([FromRoute] string museumId)
        {
            var result = await _visitorService.GetMuseumByIdAsync(museumId);
            return Ok(ApiResponse<MuseumResponseV1>.OkResponse(result, "Take museum successfully", "200"));

        }

        // Artifact Endpoints for Visitor
        [HttpGet("museums/{museumId}/artifacts")]
        [SwaggerOperation(
            Summary = "Get artifacts by museum ID",
            Description = "Retrieve a paginated list of artifacts for a specific museum, with optional filtering parameters.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetAllArtifactsByMuseumAsync(
            [FromRoute] string museumId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? artifactName = null,
            [FromQuery] string? periodTime = null,
            [FromQuery] string? areaName = null,
            [FromQuery] string? displayPositionName = null)
        {
            var result = await _visitorService.GetAllArtifactsByMuseumAsync(
                museumId, pageIndex, pageSize, artifactName, periodTime, areaName, displayPositionName);
            return Ok(ApiResponse<BasePaginatedList<ArtifactDetailsResponse>>.OkResponse(result, "Take artifacts successfully", "200"));
        }

        [HttpGet("artifacts/{artifactId}")]
        [SwaggerOperation(
            Summary = "Get artifact details by ID",
            Description = "Retrieve detailed information about a specific artifact using its ID.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetArtifactByIdAsync([FromRoute] string artifactId)
        {
            var result = await _visitorService.GetArtifactByIdAsync(artifactId);
            return Ok(ApiResponse<ArtifactDetailsResponse>.OkResponse(result, "Take artifact successfully", "200"));
        }



        //------------- Exhibition Endpoints for Visitor---------------------
        [HttpGet("exhibitions")]
        [SwaggerOperation(
            Summary = "Get all exhibitions",
            Description = "Retrieve a paginated list of exhibitions with optional filtering by exhibition name and museum ID.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetAllExhibitions(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? exhibitionName = null,
            [FromQuery] string museumId = null!)
        {
            var result = await _visitorService.GetAllExhibitions(pageIndex, pageSize, exhibitionName, museumId);
            return Ok(ApiResponse<BasePaginatedList<ExhibitionResponseV2>>.OkResponse(result, "Take exhibitions successfully", "200"));
        }


        [HttpGet("exhibitions/{exhibitionId}")]
        [SwaggerOperation(
            Summary = "Get exhibition details by ID",
            Description = "Retrieve detailed information about a specific exhibition using its ID.")]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string exhibitionId)
        {
            var result = await _visitorService.GetByIdAsync(exhibitionId);
            return Ok(ApiResponse<ExhibitionResponseV2>.OkResponse(result, "Take exhibition successfully", "200"));
        }

    }
}
