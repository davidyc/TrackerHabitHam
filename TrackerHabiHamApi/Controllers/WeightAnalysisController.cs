using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Models.Dto;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightAnalysisController : ControllerBase
    {
        private readonly IWeightAnalysisService _service;

        public WeightAnalysisController(IWeightAnalysisService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<WeightSummaryDto>> GetSummary([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var result = await _service.GetSummaryAsync(start, end);
            return Ok(result);
        }

        [HttpGet("series")]
        public async Task<ActionResult<IReadOnlyList<WeightPointDto>>> GetSeries([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var result = await _service.GetSeriesAsync(start, end);
            return Ok(result);
        }
    }
}


