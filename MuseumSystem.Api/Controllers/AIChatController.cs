using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos.AIChatDtos;
using MuseumSystem.Application.Interfaces;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/chat")]
    [ApiController]
    public class AIChatController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public AIChatController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateText([FromBody] PromptRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }
            var response = await _geminiService.SendPrompting(request.Prompt, request.MuseumId);
            return Ok(response);
        }
    }
}
