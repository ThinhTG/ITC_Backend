using Microsoft.AspNetCore.Mvc;
using ITC.Core.Contracts;
using ITC.Services.GeminiService;

namespace ITC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly ILogger<GeminiController> _logger;

        public GeminiController(IGeminiService geminiService, ILogger<GeminiController> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpPost("suggest-interpreter")]
        public async Task<IActionResult> SuggestInterpreter([FromBody] GeminiRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Contents))
                {
                    return BadRequest(new { message = "Contents cannot be empty" });
                }

                var result = await _geminiService.GetSuggestionAsync(request.Contents);
                return Ok(new GeminiResponse { Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SuggestInterpreter");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("translate")]
        public async Task<IActionResult> Translate([FromBody] GeminiRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Contents))
                {
                    return BadRequest(new { message = "Contents cannot be empty" });
                }

                var prompt = $"Translate the following text: {request.Contents}";
                var result = await _geminiService.GetSuggestionAsync(prompt);
                return Ok(new GeminiResponse { Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Translate");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] GeminiRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Contents))
                {
                    return BadRequest(new { message = "Contents cannot be empty" });
                }

                var prompt = $"Summarize the following text: {request.Contents}";
                var result = await _geminiService.GetSuggestionAsync(prompt);
                return Ok(new GeminiResponse { Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Summarize");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
} 